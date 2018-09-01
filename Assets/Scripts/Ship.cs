using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : MonoBehaviour {

	public static Ship instance;
	ShipPiece[,] m_shipPieces = new ShipPiece[5, 3]; // @todo extend
    private Vector2Int gridCenter = new Vector2Int(2, 1);
    public float shipPieceWidthInUnits;
    public float shipPieceHeightInUnits;

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
                GameObject shipPiecePrefab = main.RandomShipPiecePrefab();
                Vector2 relativePosition = ShipPiecePosition(i,j);
                GameObject shipPieceObj = Instantiate(shipPiecePrefab, relativePosition, Quaternion.identity, transform);
                m_shipPieces[i,j] = shipPieceObj.GetComponent<ShipPiece>();
            }
        }
	}

    // given a ship grid position, compute the spatial (x,y) coordinates of the corresponding ship piece
    // relative to the ship's center position
    public Vector2 ShipPiecePosition(int gridx, int gridy) {
        int gridOffsetX = gridx - gridCenter.x;
        int gridOffsetY = gridy - gridCenter.y;
        Vector2 centerPosition = gameObject.transform.position;
        float offsetx = gridOffsetX * shipPieceWidthInUnits;
        float offsety = gridOffsetY * shipPieceHeightInUnits;
        Debug.LogFormat("grid ({0},{1}) offset ({2},{3})",
                        gridx, gridy, offsetx, offsety);
        return new Vector2(offsetx, offsety);
    }
}
