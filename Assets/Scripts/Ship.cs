using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : MonoBehaviour {

    public GameObject helmPrefab;
    public GameObject highlightPrefab;
    public static readonly int maxGridWidth = 7; // must be odd
    public static readonly int maxGridHeight = 7; // must be odd
    private Vector2Int gridCenter = new Vector2Int(3, 3);
    private Vector2Int helmSlot = new Vector2Int(3, 1);
	Piece[,] m_pieces = new Piece[maxGridWidth, maxGridHeight]; // (0,0) is the bottom-leftmost possible ship piece grid coordinate. @todo extend
    GameObject[,] m_highlights = new GameObject[maxGridWidth, maxGridHeight];
    private Vector2 bottomLeftCorner;
    public float pieceWidthInUnits;
    public float pieceHeightInUnits;
    private Vector2 pieceSize;
    private Vector2Int reservedGridSlot = new Vector2Int(-1, -1); // (-1,-1) indicates no grid slot is currently reserved

    private MainController main;
	// Vector2 centerPosition = new Vector2(0f, -2.5f);

	private void Start()
	{
        GameObject manager = GameObject.Find("GameManagers");
        main = manager.GetComponent<MainController>();

        bottomLeftCorner = PiecePosition(0,0);
        bottomLeftCorner.x -= pieceWidthInUnits / 2;
        bottomLeftCorner.y -= pieceHeightInUnits / 2;
        pieceSize = new Vector2(pieceWidthInUnits, pieceHeightInUnits);

        // initialize the helm
        Vector3 helmRelativePosition = PiecePosition(helmSlot) + gameObject.transform.position;
        GameObject helmPieceObj = Instantiate(helmPrefab, helmRelativePosition, Quaternion.identity, transform);
        MakePiecePartOfMe(helmPieceObj.GetComponent<Piece>(), helmSlot);
    
        /* uncomment to fill the ship up with random pieces for testing
        for(int i = 0; i < maxGridWidth; i++) {
            for(int j = 0; j < maxGridHeight; j++) {
                if(! (i == helmSlot.x && j == helmSlot.y)) { // don't randomize the helm
                    GameObject piecePrefab = main.RandomPiecePrefab();
                    Vector3 relativePosition = PiecePosition(i,j) + gameObject.transform.position;
                    GameObject pieceObj = Instantiate(piecePrefab, relativePosition, Quaternion.identity, transform);
                    MakePiecePartOfMe(pieceObj.GetComponent<Piece>(), new Vector2Int(i,j));
                }
            }
        }
        */
	}

    // given a ship grid position, compute the spatial (x,y) coordinates of the corresponding ship piece
    // relative to the ship's center position
    public Vector3 PiecePosition(int gridx, int gridy) {
        int gridOffsetX = gridx - gridCenter.x;
        int gridOffsetY = gridy - gridCenter.y;
        Vector2 centerPosition = gameObject.transform.position;
        float offsetx = gridOffsetX * pieceWidthInUnits;
        float offsety = gridOffsetY * pieceHeightInUnits;
        // Debug.LogFormat("grid ({0},{1}) offset ({2},{3})",
        //                 gridx, gridy, offsetx, offsety);
        return new Vector3(offsetx, offsety, 1);
    }

    public Vector3 PiecePosition(Vector2Int gridSlot) {
        return PiecePosition(gridSlot.x, gridSlot.y);
    }

    // This is called when the player is done dragging, and drops a ship piece onto a valid ship grid location.
    // If we allow already-attached ship pieces to be reorganized, this will also get called on a single click: picking up a ship piece and putting it right back.
    // @return Did the piece successfully drop?
    public bool ReceivePieceFromMouse(Piece piece)
    {
        Vector2Int slot = GridSlot(piece.transform.position);
        if(slot.x >= 0 && slot.y >= 0 && slot.x < maxGridWidth && slot.y < maxGridHeight
           && piece.IsValidConnectionSlot(this, slot)) {
            MakePiecePartOfMe(piece, slot);
            piece.transform.SetParent(this.transform);
            piece.transform.position = gameObject.transform.position;
            SnapToGridSlot(piece, slot);
            return true;
        } else return false; // outside the max bounds of the ship
    }

    public void NotePieceDestroyed(Piece piece) {
        Vector2Int gridSlot = PieceGridSlot(piece);
        MakePieceNotPartOfMe(gridSlot.x, gridSlot.y);
        BreakOffAnythingNotConnectedToHelm();
    }
    
    public void MakePiecePartOfMe(Piece piece, Vector2Int gridSlot) {
        m_pieces[gridSlot.x, gridSlot.y] = piece;
        piece.Attach();
        if(piece.socket != null) {
            piece.socket.m_piece = null;
            piece.socket = null;
        }
        piece.isConnectedToHelm = true;
        if(piece.HasNorthConnector()) {
            Vector2Int northSlot = new Vector2Int(gridSlot.x, gridSlot.y+1);
            Piece otherPiece = GetPieceAt(northSlot);
            if(otherPiece != null && !otherPiece.HasSouthConnector()) {
                piece.DisableConnector("North");
                otherPiece.DisableConnector("South");
            }
        }
        if(piece.HasSouthConnector()) {
            Vector2Int southSlot = new Vector2Int(gridSlot.x, gridSlot.y-1);
            Piece otherPiece = GetPieceAt(southSlot);
            if(otherPiece != null && !otherPiece.HasNorthConnector()) {
                piece.DisableConnector("South");
                otherPiece.DisableConnector("North");
            }
        }
        if(piece.HasEastConnector()) {
            Vector2Int eastSlot = new Vector2Int(gridSlot.x+1, gridSlot.y);
            Piece otherPiece = GetPieceAt(eastSlot);
            if(otherPiece != null && !otherPiece.HasWestConnector()) {
                piece.DisableConnector("East");
                otherPiece.DisableConnector("West");
            }
        }
        if(piece.HasWestConnector()) {
            Vector2Int westSlot = new Vector2Int(gridSlot.x-1, gridSlot.y);
            Piece otherPiece = GetPieceAt(westSlot);
            if(otherPiece != null && !otherPiece.HasEastConnector()) {
                piece.DisableConnector("West");
                otherPiece.DisableConnector("East");
            }
        }
    }

    private void MakePieceNotPartOfMe(int gridx, int gridy) {
        Piece piece = GetPieceAt(gridx, gridy);
        m_pieces[gridx, gridy] = null;
        piece.isConnectedToHelm = false;
        piece.BreakOff();
        if(gridx == helmSlot.x && gridy == helmSlot.y) {
            Die();
        }
    }

    private void Die() {
        main.Lose();
    }
    
    // Note that a grid slot is currently being dragged from
    public void ReserveGridSlot(Vector3 worldPos3d)
    {
        reservedGridSlot = GridSlot(worldPos3d);
        //Debug.Log("Reserved grid slot " + reservedGridSlot);
    }

    // Note that no grid slot is currently being dragged from
    public void ReleaseGridSlot()
    {
        reservedGridSlot = new Vector2Int(-1, -1);
    }

    public void HighlightSlot(Vector2Int slot) {
        // Debug.Log("Highlighting " + slot);
        Vector3 highlightRelativePosition = PiecePosition(slot) + gameObject.transform.position;
        GameObject highlightObj = Instantiate(highlightPrefab, highlightRelativePosition, Quaternion.identity, transform);
        m_highlights[slot.x,slot.y] = highlightObj;
    }
    
    public void UnhighlightAllSlots() {
        for(int i = 0; i < maxGridWidth; i++) {
            for(int j = 0; j < maxGridHeight; j++) {
                GameObject highlight = m_highlights[i,j];
                if(highlight != null) {
                    GameObject.Destroy(highlight); // it would be more efficient to set up a whole matrix of these, then activate and deactivate them as needed, but hey, jam
                }
            }
        }
    }

    public Piece GetPieceAt (int gridx, int gridy) {
        if(gridx >= 0 && gridy >= 0 && gridx < m_pieces.GetLength(0) && gridy < m_pieces.GetLength(1)) {
            return m_pieces[gridx, gridy];
        } else {
            return null;
        }
        
    }
    public Piece GetPieceAt (Vector2Int slot) {
        return GetPieceAt(slot.x, slot.y);
    }

    public Vector2Int PieceGridSlot(Piece piece) {
        for(int i = 0; i < maxGridWidth; i++) {
            for(int j = 0; j < maxGridHeight; j++) {
                Piece candidatePiece = GetPieceAt(i,j);
                if(candidatePiece && candidatePiece == piece) {
                    return new Vector2Int(i,j);
                }
            }
        }
        Debug.LogError("Could not find grid slot for " + piece);
        return new Vector2Int(-1,-1); // indicates invalid grid slot
    }

    public void BreakOffAnythingNotConnectedToHelm() {
        // mark everything as disconnected
        for(int i = 0; i < maxGridWidth; i++) {
            for(int j = 0; j < maxGridHeight; j++) {
                Piece piece = GetPieceAt(i,j);
                if(piece != null) {
                    piece.isConnectedToHelm = false;
                }
            }
        }
        // starting with the helm, walk outwards, marking everything unconnected as connected
        MarkAsConnectedToHelm(helmSlot);
        for(int i = 0; i < maxGridWidth; i++) {
            for(int j = 0; j < maxGridHeight; j++) {
                Piece piece = GetPieceAt(i,j);
                if(piece && ! piece.isConnectedToHelm) {
                    MakePieceNotPartOfMe(i,j);
                }
            }
        }
    }

    // recursive case: mark orthogonal neighbors as connected if they have the right connection points
    private void MarkAsConnectedToHelm(Vector2Int gridSlot) {
        Piece piece = GetPieceAt(gridSlot);
        if(piece && !piece.isConnectedToHelm) { // avoid infinite recursion; only mark+recurse if you're not yet marked
            piece.isConnectedToHelm = true;
            if(piece.HasNorthConnector()) {
                Vector2Int northSlot = new Vector2Int(gridSlot.x, gridSlot.y+1);
                Piece otherPiece = GetPieceAt(northSlot);
                if(otherPiece != null && otherPiece.HasSouthConnector()) {
                    MarkAsConnectedToHelm(northSlot);
                }
            }
            if(piece.HasSouthConnector()) {
                Vector2Int southSlot = new Vector2Int(gridSlot.x, gridSlot.y-1);
                Piece otherPiece = GetPieceAt(southSlot);
                if(otherPiece != null && otherPiece.HasNorthConnector()) {
                    MarkAsConnectedToHelm(southSlot);
                }
            }
            if(piece.HasEastConnector()) {
                Vector2Int eastSlot = new Vector2Int(gridSlot.x+1, gridSlot.y);
                Piece otherPiece = GetPieceAt(eastSlot);
                if(otherPiece != null && otherPiece.HasWestConnector()) {
                    MarkAsConnectedToHelm(eastSlot);
                }
            }
            if(piece.HasWestConnector()) {
                Vector2Int westSlot = new Vector2Int(gridSlot.x-1, gridSlot.y);
                Piece otherPiece = GetPieceAt(westSlot);
                if(otherPiece != null && otherPiece.HasEastConnector()) {
                    MarkAsConnectedToHelm(westSlot);
                }
            }
        }        
    }
    
    // my max width in units
    public float MaxWidth() {
        return pieceWidthInUnits * m_pieces.GetLength(0);
    }

    // my max height in units
    public float MaxHeight() {
        return pieceHeightInUnits * m_pieces.GetLength(1);
    }

    public Vector2 MaxShipSize() {
        return new Vector2(MaxWidth(), MaxHeight());
    }
    
    // returns which (x,y) grid slot, with origin at bottom left, is the best fit for worldPos, or (-1, -1) if worldPos is outside the max ship bounds
    public Vector2Int GridSlot(Vector2 worldPos) {
        Vector2 offset = worldPos - bottomLeftCorner - (Vector2)gameObject.transform.position;
        // Debug.Log("offset = " + offset + " pieceSize = " + pieceSize + " MaxShipSize = " + MaxShipSize());
        if (offset.x < 0 || offset.x > MaxWidth()
            || offset.y < 0 || offset.y > MaxHeight())
        {
            return new Vector2Int(-1, -1); // outside the max ship bounds
        } else
        { // inside the max ship bounds
            Vector2 floatyResult = offset / pieceSize;
            int resultX = (int)Math.Floor(floatyResult.x);
            int resultY = (int)Math.Floor(floatyResult.y);
            return new Vector2Int(resultX, resultY);
        }
    }

    private void SnapToGridSlot(Piece piece, Vector2Int slot)
    {
        Vector2 offsetToOrigin = -1 * MaxShipSize() / 2;
        Vector2 pieceSizeOffset = pieceSize / 2;
        Vector2 slotOffset = slot * pieceSize;
        Vector2 offset = offsetToOrigin + slotOffset + pieceSizeOffset;
        // Debug.Log("SnapToGridSlot: " + slot + " " + offset + " " + transform.localScale + " " + offsetToOrigin + " " + slotOffset + " " + pieceSizeOffset + " ");
        piece.transform.localPosition = offset / transform.localScale;
		piece.transform.position = new Vector3(piece.transform.position.x, piece.transform.position.y, 0);
    }

}
