using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipManager : MonoBehaviour {

	public static ShipManager instance;
	ShipPiece[,] m_shipPieces = new ShipPiece[5, 11];
	Vector3 centerPosition = new Vector3(0f, -2.5f, 0f);

	void Awake()
	{
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
		
	}
}
