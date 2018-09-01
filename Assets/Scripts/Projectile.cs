using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Projectile : MonoBehaviour {

	public float m_speed = 1.5f;
	int m_damage = 1;

	void Update()
	{


		transform.Translate(Vector3.up * Time.deltaTime * m_speed);
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
