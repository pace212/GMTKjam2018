using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Path { vertical };

public class Projectile : MonoBehaviour {
	public Path m_projectilePath = Path.vertical;

	public float m_speed = 1.5f;
	int m_damage = 1;

	void Update()
	{
		switch(m_projectilePath)
		{
			case Path.vertical:
				transform.Translate(Vector3.down * Time.deltaTime * m_speed);
				break;
		}
	}

	public void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.gameObject.tag == "Ship")
		{
			collision.gameObject.GetComponent<HealthSystem>().TakeDamage(m_damage);
			Destroy(this.gameObject);
		}
	}

	public void OnTriggerExit2D(Collider2D collision)
	{
		if(collision.gameObject.tag == "Background")
		{
			Destroy(this.gameObject);
		}
	}
}
