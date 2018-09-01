using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainController : MonoBehaviour {

	public GameObject[] shipPiecePrefabs;
    public Ship playerShip;

	// Use this for initialization
	void Start () {
        GameObject playerShipObj = GameObject.Find("PlayerShip");
        playerShip = playerShipObj.GetComponent<Ship>();
	}

	public GameObject ShipPiecePrefab(int index) {
        return this.shipPiecePrefabs[index];
	}

	public GameObject RandomShipPiecePrefab() {
        int index = Mathf.FloorToInt(Random.Range(0, shipPiecePrefabs.Length));
        return this.shipPiecePrefabs[index];
	}

	// Update is called once per frame
	void Update () {
		
	}
}
