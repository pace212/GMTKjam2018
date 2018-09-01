﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PieceType { helm, sail, rudder, deck, hull, leftCannon, rightCannon, harpoon, cabin }

public class Piece : MonoBehaviour {
	public PieceType m_pieceType;
    
    // connectors are in order: North, South, East, West
	public bool[] connectors = new bool[4];

    private static System.Array pieceTypeArray = System.Enum.GetValues(typeof(PieceType));
    public bool isBeingDragged = false;
    private Vector3 originalItemPosition;
    private MainController main;
    
    // constructor: a new specific type of ship piece
    public Piece(PieceType type) {
        m_pieceType = type;
    }

	void Start()
	{
        GameObject manager = GameObject.Find("GameManagers");
        main = manager.GetComponent<MainController>();
		switch(m_pieceType)
		{
			case PieceType.helm:
				connectors = new bool[] { false, true, true, true };
				break;
			case PieceType.sail:
				connectors = new bool[] { false, false, true, true };
				break;
			case PieceType.rudder:
				connectors = new bool[] { true, false, true, true };
				break;
			case PieceType.deck:
				connectors = new bool[] { true, true, true, true };
				break;
			case PieceType.hull:
				connectors = new bool[] { false, true, false, false };
				break;
			case PieceType.leftCannon:
				connectors = new bool[] { true, true, true, false };
				break;
			case PieceType.rightCannon:
				connectors = new bool[] { true, true, false, true };
				break;
			case PieceType.harpoon:
				connectors = new bool[] { false, true, false, false };
				break;
			case PieceType.cabin:
				connectors = new bool[] { true, false, false, false };
				break;
		}
	}

	// Update is called once per frame
	void Update () {
        if(isBeingDragged) {
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 worldPos2D = new Vector2(worldPos.x, worldPos.y);
            //Debug.Log(Input.mousePosition + " " + worldPos + " " + worldPos2D);
            transform.position = worldPos2D;
        }
    }

    void OnMouseDown()
    {
        isBeingDragged = true;
        originalItemPosition = this.transform.position;
        main.playerShip.ReserveGridSlot(originalItemPosition);
    }

    void OnMouseUp()
    {
        isBeingDragged = false;

        if ( ! main.playerShip.ReceivePieceFromMouse(this))
        {
            if (this != null) { // @hack to robustify against DestroyImmediate destroying me in the middle of my OnMouseUp event
                transform.position = originalItemPosition;
            }
        }
        main.playerShip.ReleaseGridSlot();
    }

    // constructor: a new random type of ship piece
    // public Piece() {
    //     m_pieceType = (PieceType)pieceTypeArray.GetValue(Random.Range(0, pieceTypeArray.GetLength(0)));
    // }

    bool HasNorthConnector() {
        return connectors[0];
    }

    bool HasSouthConnector() {
        return connectors[1];
    }

    bool HasEastConnector() {
        return connectors[2];
    }

    bool HasWestConnector() {
        return connectors[3];
    }

	public void ShowPotentialLocations(Ship aShip)
	{

	}
            
}