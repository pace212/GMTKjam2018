using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MovementMethod { LeftToRIght, RightToLeft, DiagonalLeftToTopCenter, DiagonalRightToTopCenter, DownAndBack, LeftAndBack, RightAndBack, Stationary};

[System.Serializable]
public struct EnemyBehavior
{
	public MovementMethod m_movementMethod;

	public float m_startFiringDelay;
	public float m_cannonDelay;
	public float m_firingDelay;
	public bool m_singleSpreadShot;
	public bool m_leftShot;
    public bool m_multiSpreadShot;
    public float m_speed;
}

public class Enemy : MonoBehaviour {

	public GameObject m_bullet;
	public GameObject[] m_leftCannons;
	public GameObject[] m_rightCannons;
	public EnemyBehavior m_enemyBehavior;

	public float m_countDown;
    public bool isEvil; // true = enemy, false = player
    
	Quaternion startingRotation;
	Quaternion midRotation;
	Quaternion endingRotation;

	bool turning = false;

	void Start()
	{
		StartCoroutine(Fire(m_enemyBehavior.m_startFiringDelay));

		switch(m_enemyBehavior.m_movementMethod)
		{
			case MovementMethod.DownAndBack:
				m_countDown = m_enemyBehavior.m_startFiringDelay - 1f;
				startingRotation = transform.rotation;
				midRotation = Quaternion.Euler(transform.position.x <= 0 ? Vector3.forward * 90 : Vector3.forward * 270);
				endingRotation = Quaternion.Euler(transform.eulerAngles + (Vector3.forward * 180));
				break;
			case MovementMethod.LeftAndBack:
			case MovementMethod.RightAndBack:
				m_countDown = m_enemyBehavior.m_startFiringDelay - 0.5f;
				startingRotation = transform.rotation;
				midRotation = Quaternion.Euler(Vector3.zero);
				endingRotation = Quaternion.Euler(transform.eulerAngles + (Vector3.forward * 180));
				break;
            case MovementMethod.Stationary: // intentional no-op
                break;
		}
	}

	void Update()
	{
		m_countDown -= Time.deltaTime;

		if(m_countDown <= 0)
		{
			turning = !turning;

			m_countDown = (turning ? 2f : 999f);
		}

		if(turning)
		switch(m_enemyBehavior.m_movementMethod)
		{
			case MovementMethod.DownAndBack:
			case MovementMethod.LeftAndBack:
			case MovementMethod.RightAndBack:
				if(m_countDown >= 1)
				{	transform.rotation = Quaternion.Lerp(startingRotation, midRotation, (2 - m_countDown));	}
				else
				{	transform.rotation = Quaternion.Lerp(midRotation, endingRotation, (1 - m_countDown));	}
				break;
		}

		transform.Translate(Vector3.up * Time.deltaTime * m_enemyBehavior.m_speed);
	}

	IEnumerator Fire(float aDelay)
	{
		yield return new WaitForSeconds(aDelay);

        // if this is a ship piece, wait (yield) until it's attached to the ship
        Piece piece = GetComponent<Piece>();
        if(piece) {
            while(!piece.CanFire()) {
                yield return new WaitForSeconds(0.1f);
            }
        }

		if (!(m_enemyBehavior.m_singleSpreadShot || m_enemyBehavior.m_multiSpreadShot))
		{
			if (gameObject != null && gameObject.activeInHierarchy)
			{
				for (int i = 0; i < (m_enemyBehavior.m_leftShot ? m_leftCannons : m_rightCannons).Length; ++i)
				{
					GameObject newBullet = Instantiate<GameObject>(m_bullet,
						(m_enemyBehavior.m_leftShot ? m_leftCannons : m_rightCannons)[i].transform.position,
						(m_enemyBehavior.m_leftShot ? m_leftCannons : m_rightCannons)[i].transform.rotation);
                    if(!isEvil) { // rotate player's cannon fire northward a bit
                        newBullet.transform.rotation = Quaternion.identity;
                    }

					yield return new WaitForSeconds(m_enemyBehavior.m_cannonDelay);
				}

				StartCoroutine(Fire(m_enemyBehavior.m_firingDelay));
			}
		}
		else
		{
			if (gameObject != null && gameObject.activeInHierarchy)
			{
				for (int i = 0; i < (m_enemyBehavior.m_leftShot ? m_leftCannons : m_rightCannons).Length; ++i)
				{
					Vector3 rotation = (m_enemyBehavior.m_leftShot ? m_leftCannons : m_rightCannons)[i].transform.eulerAngles;

					for (int j = 0; j < 5; ++j)
					{
						GameObject newBullet = Instantiate<GameObject>(m_bullet,
							(m_enemyBehavior.m_leftShot ? m_leftCannons : m_rightCannons)[i].transform.position,
							Quaternion.Euler(rotation + (Vector3.forward * 15) * (j - 2)));
					}

					yield return new WaitForSeconds(m_enemyBehavior.m_cannonDelay);
				}
                if(m_enemyBehavior.m_multiSpreadShot) {
                    StartCoroutine(Fire(m_enemyBehavior.m_firingDelay));
                }                
			}
		}

		yield return null;
	}

    // I've just been reduced to zero health
    public void Die() {
        if(isEvil) {
			Destroy(this.gameObject); // @todo do better
        }
        // non-evil dying is handled in HealthSystem
    }
    
	public void OnTriggerExit2D(Collider2D collision)
	{
		if(collision.gameObject.tag == "Background")
		{
			Destroy(this.gameObject);
		}
	}
}
