using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPointAt : MonoBehaviour
{
    public bool lockRotationX;
    public bool lockRotationY;
    public bool lockRotationZ;

    private GameObject Player;

    private float visionDis = 8;

    // Start is called before the first frame update
    void Start()
    {
        Player = GameObject.FindObjectOfType<PlayerMovement>().gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (Player) if (canSeeThing(Player.transform)) TurnTowardsTarget();
    }

    private void TurnTowardsTarget()
    {
        Vector3 disToTarget = Player.transform.position - transform.position;

        Quaternion targetRotation = Quaternion.LookRotation(disToTarget, Vector3.up);

        Vector3 euler1 = transform.localEulerAngles; // get local angles BEFORE rotation
        Quaternion prevRot = transform.rotation;
        transform.rotation = targetRotation; // set rotation 
        Vector3 euler2 = transform.localEulerAngles; // get local angles AFTER rotation

        if (lockRotationX) euler2.x = euler1.x; // revert x to previous value
        if (lockRotationY) euler2.y = euler1.y; // revert y to previous value
        if (lockRotationZ) euler2.z = euler1.z; // revert z to previous value

        transform.rotation = prevRot; // revert rotation

        // animate rotation
        transform.localRotation = AnimMath.Slide(transform.localRotation, Quaternion.Euler(euler2), .01f);
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
