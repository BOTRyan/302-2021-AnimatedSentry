using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthSystem : MonoBehaviour
{
    public float health { get; private set; }
    public float healthMax = 100;
    public ParticleSystem prefabExplosion;
    public GameObject prefabDamageFloatie;
    public TMPro.TextMeshProUGUI healthDis;
    public GameObject deathScreen;
    public GameObject HUD;

    private void Start()
    {
        health = healthMax;
    }

    private void Update()
    {
        if (gameObject.GetComponent<PlayerMovement>())
        {
            healthDis.text = "Health: " + health;
        }
    }

    public void TakeDamage(float amt)
    {
        if (amt <= 0) return;

        health -= amt;

        if (gameObject.GetComponent<EnemyTargeting>())
        {
            Instantiate(prefabDamageFloatie, new Vector3(transform.position.x, transform.position.y + 2, transform.position.z), gameObject.transform.rotation);
        }

        if (health <= 0)
        {
            health = 0;
            Die();
        }
    }
    private void Die()
    {

        if (gameObject.GetComponent<PlayerMovement>())
        {
            deathScreen.SetActive(true);
            HUD.SetActive(false);
            Cursor.lockState = CursorLockMode.None;
            Destroy(gameObject);
            //StartCoroutine("ShowMenu");
        }
        if (gameObject.GetComponent<EnemyTargeting>())
        {
            //gameObject.GetComponent<EnemyTargeting>().Die();
            Instantiate(prefabExplosion, gameObject.transform.position, gameObject.transform.rotation);
            Destroy(gameObject);
        }
    }

    IEnumerator ShowMenu()
    {
        yield return new WaitForSeconds(2);

    }
}
