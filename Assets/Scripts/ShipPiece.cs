using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ShipPieceType { helm, sail, rudder, deck, hull, leftCannon, rightCannon, harpoon, cabin }

public class ShipPiece : MonoBehaviour {
	public ShipPieceType m_shipPieceType;
    private static System.Array shipPieceTypeArray = System.Enum.GetValues(typeof(ShipPieceType));
    
    // connectors are in order: North, South, East, West
	public bool[] connectors = new bool[4];

	void Start()
	{
		switch(m_shipPieceType)
		{
			case ShipPieceType.helm:
				connectors = new bool[] { false, true, true, true };
				break;
			case ShipPieceType.sail:
				connectors = new bool[] { false, false, true, true };
				break;
			case ShipPieceType.rudder:
				connectors = new bool[] { true, false, true, true };
				break;
			case ShipPieceType.deck:
				connectors = new bool[] { true, true, true, true };
				break;
			case ShipPieceType.hull:
				connectors = new bool[] { false, true, false, false };
				break;
			case ShipPieceType.leftCannon:
				connectors = new bool[] { true, true, true, false };
				break;
			case ShipPieceType.rightCannon:
				connectors = new bool[] { true, true, false, true };
				break;
			case ShipPieceType.harpoon:
				connectors = new bool[] { false, true, false, false };
				break;
			case ShipPieceType.cabin:
				connectors = new bool[] { true, false, false, false };
				break;
		}
	}

    // constructor: a new specific type of ship piece
    public ShipPiece(ShipPieceType type) {
        m_shipPieceType = type;
    }

    // constructor: a new random type of ship piece
    // public ShipPiece() {
    //     m_shipPieceType = (ShipPieceType)shipPieceTypeArray.GetValue(Random.Range(0, shipPieceTypeArray.GetLength(0)));
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

	// public void ShowPotentialLocations(Ship aShip)
	// {

	// }
    // so, the last thing I was working on was a ship manager script that will hold the configuration of the ship in a 2d array
    
    // after getting that working, I was going to get the menu on the right side of the screen to work as slots that hold new ship pieces that the player can drag and drop into the scene
            
}
