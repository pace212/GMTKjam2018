using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainController : MonoBehaviour {

	public GameObject[] piecePrefabs;
    public Ship playerShip;
    public float secondsBetweenPieceDrops;
    private float secondsSinceLastPieceDrop = 0;

    private AudioSource audioSource;

	// Use this for initialization
	void Start () {
        audioSource = GetComponent<AudioSource>();
        GameObject playerShipObj = GameObject.Find("PlayerShip");
        playerShip = playerShipObj.GetComponent<Ship>();
	}

	public GameObject PiecePrefab(int index) {
        return this.piecePrefabs[index];
	}

	public GameObject RandomPiecePrefab() {
        int index = Mathf.FloorToInt(Random.Range(0, piecePrefabs.Length));
        return this.piecePrefabs[index];
	}

	// Update is called once per frame
	void Update () {
        secondsSinceLastPieceDrop += Time.deltaTime;
        if(secondsSinceLastPieceDrop >= secondsBetweenPieceDrops) {
            DropNewPiece();
        }
	}

    void DropNewPiece () {
        secondsSinceLastPieceDrop = 0;
        foreach (Socket socket in FindObjectsOfType<Socket>())
            if(! socket.m_piece) {
                socket.InstantiatePiece(RandomPiecePrefab());
                return;
            }
    }
}
