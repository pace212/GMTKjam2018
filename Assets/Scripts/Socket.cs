using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Socket : MonoBehaviour {

    public Piece m_piece;
    public int pieceInitialSortOrder = 26; // one more than the Socket sort order
    private Vector2 pieceScale = new Vector2(0.5f, 0.5f);
    private MainController main;

	// Use this for initialization
	void Start () {
        GameObject manager = GameObject.Find("GameManagers");
        main = manager.GetComponent<MainController>();
        GameObject piecePrefab = main.RandomPiecePrefab();
        GameObject pieceObj = Instantiate(piecePrefab, transform);
        pieceObj.transform.localScale = pieceScale;
        SpriteRenderer sprite = pieceObj.GetComponent<SpriteRenderer>();
        if (sprite)
        {
            sprite.sortingOrder = pieceInitialSortOrder;
        }
		m_piece = pieceObj.GetComponent<Piece>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
