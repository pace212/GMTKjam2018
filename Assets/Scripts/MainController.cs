using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainController : MonoBehaviour {

	public GameObject[] piecePrefabs;
    public Ship playerShip;

	// Use this for initialization
	void Start () {
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
		
	}
}
