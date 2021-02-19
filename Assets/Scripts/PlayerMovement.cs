using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    private CharacterController pawn;
    public Transform leg1;
    public Transform leg2;
    public float walkSpeed = 5;

    private Camera cam;
    Vector3 inputDirection = new Vector3();

    private float verticalVelocity = 0;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        pawn = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        MovePlayer();
        WiggleLegs();
    }

    private void WiggleLegs()
    {
        float degrees = 35;
        float speed = 10;

        Vector3 inputDirLocal = transform.InverseTransformDirection(inputDirection);
        Vector3 axis = Vector3.Cross(inputDirLocal, Vector3.up);

        float alignment = Vector3.Dot(inputDirLocal, Vector3.forward);

        alignment = Mathf.Abs(alignment); // flips negative numbers

        degrees *= AnimMath.Lerp(0.25f, 1, alignment); // decrease 'degrees' when strafing

        float wave = Mathf.Sin(Time.time * speed) * degrees;

        leg1.localRotation = AnimMath.Slide(leg1.localRotation, Quaternion.AngleAxis(wave, axis), 0.001f);
        leg2.localRotation = AnimMath.Slide(leg2.localRotation, Quaternion.AngleAxis(-wave, axis), 0.001f);
    }

    private void MovePlayer()
    {
        float h = Input.GetAxis("Horizontal"); // strafing?
        float v = Input.GetAxis("Vertical"); // forward / backward
        float z = Input.GetAxis("Jump"); // jump

        bool isTryingToMove = (h != 0 || v != 0);

        if (z != 0 && pawn.isGrounded)
        {
            verticalVelocity = -5;
        }

        if (isTryingToMove)
        {
            float camYaw = cam.transform.eulerAngles.y;
            transform.rotation = AnimMath.Slide(transform.rotation, Quaternion.Euler(0, camYaw, 0), 0.02f);
        }

        inputDirection = transform.forward * v + transform.right * h;
        if (inputDirection.sqrMagnitude > 1) inputDirection.Normalize();

        // apply gravity
        verticalVelocity += 10 * Time.deltaTime;

        // adds lateral movement to vertical movement
        Vector3 moveDelta = inputDirection * walkSpeed + verticalVelocity * Vector3.down;

        // move pawn
        pawn.Move(moveDelta * Time.deltaTime);

        if (pawn.isGrounded)
        {
            verticalVelocity = 0;
        }
    }
}
