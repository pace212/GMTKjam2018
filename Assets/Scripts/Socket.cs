using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Socket : MonoBehaviour {

    public GameObject initialPiecePrefab;
    public Piece m_piece;
    // Socket sort order = 25
    public int highlightInitialSortOrder = 26;
    public int pieceInitialSortOrder = 27;
    private Vector3 pieceScale = new Vector3(0.67f, 0.67f, 0.67f);
    private MainController main;
    private GameObject myHighlight;

	// Use this for initialization
	void Start () {
        GameObject manager = GameObject.Find("GameManagers");
        main = manager.GetComponent<MainController>();
        if(initialPiecePrefab) {
            InstantiatePiece(initialPiecePrefab);
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void InstantiatePiece(GameObject piecePrefab) {
        GameObject pieceObj = Instantiate(piecePrefab, transform);
        pieceObj.transform.localScale = pieceScale;
        SpriteRenderer sprite = pieceObj.GetComponent<SpriteRenderer>();
        if (sprite != null) {
            sprite.sortingOrder = pieceInitialSortOrder;
        }
        m_piece = pieceObj.GetComponent<Piece>();
        m_piece.socket = this;
    }
    
    void OnMouseEnter() {
        Highlight();
    }

    void OnMouseExit() {
        Unhighlight();
    }

    public void Highlight() {
        if(m_piece){
            Vector3 highlightRelativePosition = gameObject.transform.position;
            GameObject highlightObj = Instantiate(main.playerShip.highlightPrefab, highlightRelativePosition, Quaternion.identity, transform);
            myHighlight = highlightObj;
            SpriteRenderer sprite = highlightObj.GetComponent<SpriteRenderer>();
            if (sprite != null) {
                sprite.sortingOrder = highlightInitialSortOrder;
            }
        }
    }

    public void Unhighlight() {
        if(myHighlight != null) {
            GameObject.Destroy(myHighlight);
        }
    }
}
