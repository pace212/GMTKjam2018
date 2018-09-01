using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[System.Serializable]
public struct EnemySpawn
{
	public GameObject m_enemy;
	public float m_spawnTime;
	public Vector3 m_spawnLocation;
	public Vector3 m_eulerRotation;
	public int m_enemyBehaviorIndex;
}

public class EnemyManager : MonoBehaviour {

	public static EnemyManager instance;

	public EnemyBehavior[] m_enemyTypes;
	public List<EnemySpawn> m_enemySpawns;

	float countDown;

	void Awake ()
	{
		if(instance == null)
		{
			instance = this;
			DontDestroyOnLoad(gameObject);
			Init();
		}
		else
		{	Destroy(gameObject);	}
	}

	private void Init()
	{
		if(m_enemySpawns.Count > 0)
		{	countDown = m_enemySpawns[0].m_spawnTime;	}
		else
		{	countDown = 999f;	}
	}

	void Update()
	{
		countDown -= Time.deltaTime;

		if (countDown <= 0)
		{
			bool enemySpawnLoop = true;

			while(enemySpawnLoop)
			{
				if(Time.time >= m_enemySpawns[0].m_spawnTime)
				{
					GameObject newEnemy = Instantiate<GameObject>(m_enemySpawns[0].m_enemy,
						m_enemySpawns[0].m_spawnLocation,
						Quaternion.Euler(m_enemySpawns[0].m_eulerRotation));
					newEnemy.GetComponent<Enemy>().m_enemyBehavior = m_enemyTypes[m_enemySpawns[0].m_enemyBehaviorIndex];

					m_enemySpawns.RemoveAt(0);

					if(m_enemySpawns.Count == 0)
					{	enemySpawnLoop = false;	}
				}
				else
				{	enemySpawnLoop = false;	}
			}

			if (m_enemySpawns.Count > 0)
			{ countDown = m_enemySpawns[0].m_spawnTime; }
			else
			{ countDown = 999f; }
		}

	}
}
