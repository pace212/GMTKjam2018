using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MovementMethod { LeftToRIght, RightToLeft, DiagnalToTopCenter };

[System.Serializable]
public struct EnemyBehavior
{
	public MovementMethod m_movementMethod;

	public float m_startFiringDelay;
	public float m_cannonDelay;
	public float m_firingDelay;
}

public class Enemy : MonoBehaviour {

	public GameObject m_bullet;
	public GameObject[] m_cannons;
	public EnemyBehavior m_enemyBehavior;

	public float m_speed = 1f;

	void Start()
	{
		StartCoroutine(Fire(m_enemyBehavior.m_startFiringDelay));
	}

	void Update()
	{
		switch(m_enemyBehavior.m_movementMethod)
		{
			case MovementMethod.LeftToRIght:
				transform.Translate(Vector3.right * Time.deltaTime * m_speed);
				break;
		}
	}

	IEnumerator Fire(float aDelay)
	{
		yield return new WaitForSeconds(aDelay);

		if(gameObject != null && gameObject.activeInHierarchy)
		{
			for(int i = 0; i < m_cannons.Length; ++i)
			{
				GameObject newBullet = Instantiate<GameObject>(m_bullet,
					m_cannons[i].transform.position,
					m_cannons[i].transform.rotation);

				yield return new WaitForSeconds(m_enemyBehavior.m_cannonDelay);
			}

			StartCoroutine(Fire(m_enemyBehavior.m_firingDelay));
		}

		yield return null;
	}

	public void OnTriggerExit2D(Collider2D collision)
	{
		if(collision.gameObject.tag == "Background")
		{
			Destroy(this.gameObject);
		}
	}
}
