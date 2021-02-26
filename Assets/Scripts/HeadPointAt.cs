using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadPointAt : MonoBehaviour
{

    private PlayerTargeting playerTargeting;
    public Transform cameraTarg;

    public bool lockRotationX;
    public bool lockRotationY;
    public bool lockRotationZ;

    public Quaternion startingRotation;

    // Start is called before the first frame update
    void Start()
    {
        playerTargeting = GetComponentInParent<PlayerTargeting>();
        startingRotation = transform.localRotation;
    }

    // Update is called once per frame
    void Update()
    {
        TurnTowardsTarget();
    }

    private void TurnTowardsTarget()
    {
        if (playerTargeting && playerTargeting.target && playerTargeting.wantsToTarget)
        {
            Vector3 disToTarget = playerTargeting.target.position - transform.position;

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
        else
        {
            // figure out bone rotation, no target:
            transform.localRotation = AnimMath.Slide(transform.localRotation, Quaternion.Euler(0, cameraTarg.localRotation.y * 180 / Mathf.PI, 0), 0.001f);
        }

    }
}
