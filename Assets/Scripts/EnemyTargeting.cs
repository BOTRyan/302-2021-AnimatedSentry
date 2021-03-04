using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTargeting : MonoBehaviour
{
    private float visionDis = 8;

    private float shootCooldown = 0;

    private float roundsPerSecond = 1f;
    private float bulletSpeed = 10;

    public ParticleSystem prefabMuzzleFlash;
    public GameObject bulletPrefab;

    private GameObject Player;
    public Transform head;
    public Transform barrel;
    private Vector3 destination;

    // Start is called before the first frame update
    void Start()
    {
        Player = GameObject.FindObjectOfType<PlayerMovement>().gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (shootCooldown > 0) shootCooldown -= Time.deltaTime;

        DoAttack();
    }

    public void Die()
    {
        Destroy(gameObject);
    }

    private void DoAttack()
    {
        if (shootCooldown > 0) return;
        if (Player == null) return;
        if (Player.GetComponent<PlayerMovement>().isDead) return;
        if (!canSeeThing(Player.transform)) return;

        // resets cooldown:
        if (barrel)
        {
            Instantiate(prefabMuzzleFlash, barrel.position, barrel.rotation);
            var bullet = Instantiate(bulletPrefab, barrel.position, barrel.rotation);
            bullet.GetComponent<Rigidbody>().velocity = (Player.transform.position - barrel.position).normalized * bulletSpeed;
        }
        shootCooldown = 1 / roundsPerSecond;
    }

    private bool canSeeThing(Transform thing)
    {
        if (!thing) return false; // uh... error

        Vector3 vToThing = thing.position - transform.position; // 

        // check distance:
        if (vToThing.sqrMagnitude > visionDis * visionDis) return false; // too far to see

        // TODO: check occlusion

        return true;
    }
}
