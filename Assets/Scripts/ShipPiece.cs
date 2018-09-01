using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ShipPieceType { helm, sail, rudder, deck, hull, leftCannon, rightCannon, harpoon, cabin }

public class ShipPiece : MonoBehaviour {
	public ShipPieceType m_shipPieceType;

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

	public void ShowPotentialLocations(Ship aShip)
	{

	}
}
