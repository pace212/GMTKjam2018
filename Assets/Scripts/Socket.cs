using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Socket : MonoBehaviour {

    public ShipPiece m_shipPiece;
    public int shipPieceInitialSortOrder = 26; // one more than the Socket sort order
    private Vector2 shipPieceScale = new Vector2(0.5f, 0.5f);
    private MainController main;

	// Use this for initialization
	void Start () {
        GameObject manager = GameObject.Find("GameManagers");
        main = manager.GetComponent<MainController>();
        GameObject shipPiecePrefab = main.RandomShipPiecePrefab();
        GameObject shipPieceObj = Instantiate(shipPiecePrefab, transform);
        shipPieceObj.transform.localScale = shipPieceScale;
        SpriteRenderer sprite = shipPieceObj.GetComponent<SpriteRenderer>();
        if (sprite)
        {
            sprite.sortingOrder = shipPieceInitialSortOrder;
        }
		m_shipPiece = shipPieceObj.GetComponent<ShipPiece>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
