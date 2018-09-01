using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : MonoBehaviour {

	public static Ship instance;
	Piece[,] m_pieces = new Piece[5, 3]; // @todo extend
    private Vector2Int gridCenter = new Vector2Int(2, 1);
    public float pieceWidthInUnits;
    public float pieceHeightInUnits;
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
        
        for(int i = 0; i < 5; i++) {
            for(int j = 0; j < 3; j++) {
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

    // This is called when the player is done dragging, and drops a ship piece onto the edge of the ship.
    // If we allow already-attached ship pieces to be reorganized, this will also get called on a single click: picking up a ship piece and putting it right back.
    // @return Did the piece successfully drop?
    public bool ReceivePieceFromMouse(Piece piece)
    {
        return false; // stub
    }
    
    public void ReserveGridSlot(Vector3 worldPos3d)
    {
        reservedGridSlot = GridSlot(worldPos3d);
        //Debug.Log("Reserved grid slot " + reservedGridSlot);
    }

    public void ReleaseGridSlot()
    {
        reservedGridSlot = new Vector2Int(-1, -1);
    }
    
    public Vector2Int GridSlot(Vector3 worldPos3d)
        // returns which (x,y) grid slot, with origin at bottom left, is the best fit for worldPos, or (-1, -1) if worldPos is outside the max ship bounds
    {
        return GridSlot(worldPos3d);
    }
    public Vector2Int GridSlot(Vector2 worldPos) {
        // returns which (x,y) grid slot, with origin at bottom left, is the best fit for worldPos, or (-1, -1) if worldPos is outside the max ship bounds
        // Vector2 offset = worldPos - bottomLeftCorner;
        // //Debug.Log("offset = " + offset + " slotsize = " + slotSize + " spriteSize = " + spriteSize);
        // if (offset.x < 0 || offset.x > spriteSize.x
        //     || offset.y < 0 || offset.y > spriteSize.y)
        // {
        //     return new Vector2Int(-1, -1); // outside the max ship bounds
        // } else
        // { // inside the max ship bounds
        //     Vector2 floatyResult = offset / slotSize;
        //     int resultX = (int)Math.Floor(floatyResult.x);
        //     int resultY = (int)Math.Floor(floatyResult.y);
        //     return new Vector2Int(resultX, resultY);
        // }
        return new Vector2Int(-1,-1); // stub
    }

    // private void SnapToGridSlot(GameObject piece, Vector2Int slot)
    // {
    //     Vector2 offsetToOrigin = -1 * spriteSize / 2;
    //     Vector2 pieceSizeOffset = slotSize / 2;
    //     Vector2 slotOffset = slot * slotSize;
    //     Vector2 offset = offsetToOrigin + slotOffset + pieceSizeOffset;
    //     //Debug.Log("SnapToGridSlot: " + offsetToOrigin + " " + slotOffset + " " + pieceSizeOffset);
    //     piece.transform.localPosition = ZeroZ(offset);
    // }

}
