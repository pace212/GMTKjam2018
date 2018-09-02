using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainController : MonoBehaviour {

	public GameObject[] piecePrefabs;
    public Ship playerShip;
    public float secondsBetweenPieceDrops;
    public float secondsUntilVictorie;
    public bool gameOver = false;
    public bool isVictorieuouous = false;

    private GameObject victorieMsg;
    public GameObject lockeredMsg;
    public AudioClip loseSound;
    public float loseSoundVolume;
    private float secondsSinceLastPieceDrop = 0;
    private AudioSource audioSource;

	// Use this for initialization
	void Start () {
        gameOver = false;
        victorieMsg = GameObject.Find("Victorie");
        victorieMsg.SetActive(false);
        lockeredMsg = GameObject.Find("Lockered");
        lockeredMsg.SetActive(false);
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
        } else {
            secondsUntilVictorie -= Time.deltaTime;
            if(secondsUntilVictorie <= 0) {
                Win();
            }
            secondsSinceLastPieceDrop += Time.deltaTime;
            if(secondsSinceLastPieceDrop >= secondsBetweenPieceDrops) {
                DropNewPiece();
            }
        }
	}

    void Restart() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void Win() {
        gameOver = true;
        isVictorieuouous = true;
        victorieMsg.SetActive(true);
        playerShip.ReplaceHelmWithSail();
    }

    public void Lose() {
        gameOver = true;
        isVictorieuouous = false;
        lockeredMsg.SetActive(true);
        audioSource.PlayOneShot(loseSound, loseSoundVolume);
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
            if(isVictorieuouous) {
                GUIStyle gameOverStyle = new GUIStyle();
                gameOverStyle.normal.textColor = Color.yellow;
                gameOverStyle.fontSize = 32;
                gameOverStyle.fontStyle = FontStyle.Bold;
                gameOverStyle.alignment = TextAnchor.LowerCenter;
                GUI.Label(new Rect(Screen.width / 2, Screen.height / 2 + 100, 100, 20), "Press\nARRRRRRRRR\nTO RESTART)", gameOverStyle);
            } else {
                GUIStyle gameOverStyle = new GUIStyle();
                gameOverStyle.normal.textColor = Color.red;
                gameOverStyle.fontSize = 32;
                gameOverStyle.fontStyle = FontStyle.Bold;
                gameOverStyle.alignment = TextAnchor.LowerCenter;
                GUI.Label(new Rect(Screen.width / 2, Screen.height / 2+ 100, 100, 20), "Press\nARRRRRRRRR\nTO RESTART)", gameOverStyle);
            }
		}
	}

}
