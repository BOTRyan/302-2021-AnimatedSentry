using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{

    private float lifeTime = 0;

    private void Update()
    {
        lifeTime += Time.deltaTime;
        if (lifeTime >= 5) Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerMovement player = other.GetComponent<PlayerMovement>();
        if (player)
        {
            HealthSystem playerHealth = player.GetComponent<HealthSystem>();
            if (playerHealth)
            {
                playerHealth.TakeDamage(10);
            }
            Destroy(gameObject);
        }
    }
}
