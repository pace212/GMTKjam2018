using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : MonoBehaviour {

	public static Ship instance;
	Piece[,] m_pieces = new Piece[5, 3]; // (0,0) is the bottom-leftmost possible ship piece grid coordinate. @todo extend
    private Vector2Int gridCenter = new Vector2Int(2, 1);
    private Vector2 bottomLeftCorner;
    public float pieceWidthInUnits;
    public float pieceHeightInUnits;
    private Vector2 pieceSize;
    private Vector2Int reservedGridSlot = new Vector2Int(-1, -1); // (-1,-1) indicates no grid slot is currently reserved

    private MainController main;
	// Vector2 centerPosition = new Vector2(0f, -2.5f);

	void Awake()
	{
        // What is this doing? Making sure there's only ever one instance of Ship? -Pace
		if(instance == null)
		{
			instance = this;
			GameObject.DontDestroyOnLoad(gameObject);
			Init();
		}
		else
		{	GameObject.Destroy(gameObject);	}
	}

	private void Init()
	{
        GameObject manager = GameObject.Find("GameManagers");
        main = manager.GetComponent<MainController>();

        bottomLeftCorner = PiecePosition(0,0);
        bottomLeftCorner.x -= pieceWidthInUnits / 2;
        bottomLeftCorner.y -= pieceHeightInUnits / 2;
        pieceSize = new Vector2(pieceWidthInUnits, pieceHeightInUnits);
        
        for(int i = 1; i < 4; i++) {
            for(int j = 1; j < 2; j++) {
                GameObject piecePrefab = main.RandomPiecePrefab();
                Vector2 relativePosition = PiecePosition(i,j);
                GameObject pieceObj = Instantiate(piecePrefab, relativePosition, Quaternion.identity, transform);
                m_pieces[i,j] = pieceObj.GetComponent<Piece>();
            }
        }
	}

    // given a ship grid position, compute the spatial (x,y) coordinates of the corresponding ship piece
    // relative to the ship's center position
    public Vector2 PiecePosition(int gridx, int gridy) {
        int gridOffsetX = gridx - gridCenter.x;
        int gridOffsetY = gridy - gridCenter.y;
        Vector2 centerPosition = gameObject.transform.position;
        float offsetx = gridOffsetX * pieceWidthInUnits;
        float offsety = gridOffsetY * pieceHeightInUnits;
        // Debug.LogFormat("grid ({0},{1}) offset ({2},{3})",
        //                 gridx, gridy, offsetx, offsety);
        return new Vector2(offsetx, offsety);
    }

    // This is called when the player is done dragging, and drops a ship piece onto a valid ship grid location.
    // If we allow already-attached ship pieces to be reorganized, this will also get called on a single click: picking up a ship piece and putting it right back.
    // @return Did the piece successfully drop?
    public bool ReceivePieceFromMouse(Piece piece)
    {
        return false; // stub
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

    // my max width in units
    public float MaxWidth() {
        return pieceWidthInUnits * m_pieces.GetLength(0);
    }

    // my max height in units
    public float MaxHeight() {
        return pieceHeightInUnits * m_pieces.GetLength(1);
    }
    
    // returns which (x,y) grid slot, with origin at bottom left, is the best fit for worldPos, or (-1, -1) if worldPos is outside the max ship bounds
    public Vector2Int GridSlot(Vector2 worldPos) {
        Vector2 offset = worldPos - bottomLeftCorner;
        //Debug.Log("offset = " + offset + " piecesize = " + pieceSize + " spriteSize = " + spriteSize);
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

    // private void SnapToGridSlot(GameObject piece, Vector2Int slot)
    // {
    //     Vector2 offsetToOrigin = -1 * spriteSize / 2;
    //     Vector2 pieceSizeOffset = pieceSize / 2;
    //     Vector2 slotOffset = slot * pieceSize;
    //     Vector2 offset = offsetToOrigin + slotOffset + pieceSizeOffset;
    //     //Debug.Log("SnapToGridSlot: " + offsetToOrigin + " " + slotOffset + " " + pieceSizeOffset);
    //     piece.transform.localPosition = ZeroZ(offset);
    // }

}
