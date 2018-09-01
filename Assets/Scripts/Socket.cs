using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Socket : MonoBehaviour {

    public ShipPiece m_shipPiece;
    private MainController main;

	// Use this for initialization
	void Start () {
        GameObject manager = GameObject.Find("GameManagers");
        main = manager.GetComponent<MainController>();
        GameObject shipPiecePrefab = main.RandomShipPiecePrefab();
        GameObject shipPieceObj = Instantiate(shipPiecePrefab, transform);
		m_shipPiece = shipPieceObj.GetComponent<ShipPiece>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
