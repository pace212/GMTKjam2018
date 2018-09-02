using System.Collections;
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
    public Socket socket;
    public bool isConnectedToHelm = false;
    private bool isDriftingAway = false;
    public float driftAwaySpeed = 1; // the speed at which broken-off ship parts drift down off the screen
    private float driftAwayWobble = 0.5f; // how much back-and-forth wobble the ship parts wobble as they drift down off the screen
    private Vector2 velocity = new Vector2(0,0);
    public AudioClip attachSound;
    private AudioSource audioSource;
    
    // constructor: a new specific type of ship piece
    public Piece(PieceType type) {
        m_pieceType = type;
    }

	void Start()
	{
        GameObject manager = GameObject.Find("GameManagers");
        main = manager.GetComponent<MainController>();
        audioSource = GetComponent<AudioSource>();
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
        if(isDriftingAway) {
            Vector2 velocity = new Vector2(Random.Range(-driftAwayWobble, driftAwayWobble), -driftAwaySpeed);
            transform.position += (Vector3)velocity * Time.deltaTime;
            transform.localScale *= 0.998f; // yeah, I know this should be dependent on Time.deltaTime, but nothing bad happens if it's not
        }
    }

    void OnMouseEnter() {
        if(socket && !isBeingDragged) {
            socket.Highlight();
        }
    }

    void OnMouseExit() {
        if(socket && !isBeingDragged) {
            socket.Unhighlight();
        }
    }

    void OnMouseDown()
    {
        if(socket && !isDriftingAway) { // remove socket if we allow repositioning ship pieces
            // Debug.Log("mouse down" + " " + socket + " " + isDriftingAway + " " + isBeingDragged);
            isBeingDragged = true;
            if(socket != null) {
                socket.Unhighlight();
            }
            originalItemPosition = this.transform.position;
            main.playerShip.ReserveGridSlot(originalItemPosition);
            ShowPotentialLocations(main.playerShip);
        }
    }

    void OnMouseUp()
    {
        if(socket && !isDriftingAway) { // remove socket if we allow repositioning ship pieces
            // Debug.Log("mouse up" + " " + socket + " " + isDriftingAway + " " + isBeingDragged);
            isBeingDragged = false;
            if(socket != null) {
                socket.Unhighlight();
            }

            if ( ! main.playerShip.ReceivePieceFromMouse(this))
            {
                if (this != null) { // @hack to robustify against DestroyImmediate destroying me in the middle of my OnMouseUp event
                    transform.position = originalItemPosition;
                }
            }
            main.playerShip.ReleaseGridSlot();
            main.playerShip.UnhighlightAllSlots();
        }
    }

    // I am now attached to the ship
    public void Attach() {
        gameObject.tag = "Ship";
        if(audioSource) {
            audioSource.PlayOneShot(attachSound);
        }
    }
    
    // I break off of the ship
    public void BreakOff() {
        gameObject.tag = "Background";
        main.playerShip.ReleaseGridSlot();
        isDriftingAway = true;
        isBeingDragged = false;
        if(socket != null) {
            socket.Unhighlight();
        }
    }        
    
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
        for(int i = 0; i < Ship.maxGridWidth; i++) {
            for(int j = 0; j < Ship.maxGridHeight; j++) {
                Vector2Int slot = new Vector2Int(i,j);
                if(IsValidConnectionSlot(aShip, slot)){
                    aShip.HighlightSlot(slot);
                }
            }
        }
	}

    public bool IsValidConnectionSlot(Ship aShip, Vector2Int gridSlot) {
        if(aShip.GetPieceAt(gridSlot) != null) {
            return false; // this slot is already full
        }
        if(HasNorthConnector()) {
            Vector2Int northSlot = new Vector2Int(gridSlot.x, gridSlot.y+1);
            Piece shipPiece = aShip.GetPieceAt(northSlot);
            if(shipPiece != null && shipPiece.HasSouthConnector()) {
                return true;
            }
        }
        if(HasSouthConnector()) {
            Vector2Int southSlot = new Vector2Int(gridSlot.x, gridSlot.y-1);
            Piece shipPiece = aShip.GetPieceAt(southSlot);
            if(shipPiece != null && shipPiece.HasNorthConnector()) {
                return true;
            }
        }
        if(HasEastConnector()) {
            Vector2Int eastSlot = new Vector2Int(gridSlot.x+1, gridSlot.y);
            Piece shipPiece = aShip.GetPieceAt(eastSlot);
            if(shipPiece != null && shipPiece.HasWestConnector()) {
                return true;
            }
        }
        if(HasWestConnector()) {
            Vector2Int westSlot = new Vector2Int(gridSlot.x-1, gridSlot.y);
            Piece shipPiece = aShip.GetPieceAt(westSlot);
            if(shipPiece != null && shipPiece.HasEastConnector()) {
                return true;
            }
        }
        return false;
    }
}
