using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainController : MonoBehaviour {

	public GameObject[] piecePrefabs;
    public Ship playerShip;
    public float secondsBetweenPieceDrops;
    public bool gameOver = false;
    
    private float secondsSinceLastPieceDrop = 0;
    private AudioSource audioSource;

	// Use this for initialization
	void Start () {
        gameOver = false;
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
        if (gameOver)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                Restart();
            }
        }
        secondsSinceLastPieceDrop += Time.deltaTime;
        if(secondsSinceLastPieceDrop >= secondsBetweenPieceDrops) {
            DropNewPiece();
        }
	}

    void Restart() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    
    void DropNewPiece () {
        secondsSinceLastPieceDrop = 0;
        foreach (Socket socket in FindObjectsOfType<Socket>())
            if(! socket.m_piece) {
                socket.InstantiatePiece(RandomPiecePrefab());
                return;
            }
    }

    void OnGUI() {
		if(gameOver) {
			GUIStyle gameOverStyle = new GUIStyle();
			gameOverStyle.normal.textColor = Color.red;
			gameOverStyle.fontSize = 64;
			gameOverStyle.fontStyle = FontStyle.Bold;
			gameOverStyle.alignment = TextAnchor.LowerCenter;
			GUI.Label(new Rect(Screen.width / 2, Screen.height / 2, 100, 20), "ARRRRRRRRR\nTO RESTART\n(Jake, please fix this)", gameOverStyle);
		}
	}

}
