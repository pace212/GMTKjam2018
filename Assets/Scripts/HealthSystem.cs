using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem : MonoBehaviour {
	public enum DamageState { vulnerable, invulnerable, enemy}

	public SpriteRenderer m_spriteRender;
	public DamageState m_damageState;

	public float m_deathTimer = 1.5f;
    public int m_maxHealth = 3;
    public int m_health;

    void Start()
    {
        m_health = m_maxHealth;
    }

	public void AddHealth(int aHealth)
    {
        m_health += aHealth;
		
        if(m_health > m_maxHealth)
        {   m_health = m_maxHealth; }
    }

    public void TakeDamage(int aDamage)
    {
        m_health -= aDamage;

		if(m_health <= 0)
		{
			m_health = 0;
			StartCoroutine("Dead");
		}
		else if(aDamage >= 1)
		{
			StartCoroutine("DamageVisual");
		}
    }

	IEnumerator DamageVisual()
	{
		Color originalColor = m_spriteRender.color;

		for(int i = 0; i < 10; ++i)
		{
			if (i % 2 == 0)
				m_spriteRender.color = Color.red;
			else
				m_spriteRender.color = Color.gray;

			yield return new WaitForSeconds(0.005f * (10f + i));
		}

		m_spriteRender.color = originalColor;

		yield return null;
	}

	IEnumerator Dead()
	{
		if(m_spriteRender != null)
		{
			m_spriteRender.color = Color.red;

			yield return new WaitForSeconds(m_deathTimer);

			Destroy(this.gameObject);
		}

		yield return null;
	}
}
