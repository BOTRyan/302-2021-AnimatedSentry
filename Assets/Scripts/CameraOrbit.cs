using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraOrbit : MonoBehaviour
{

    public PlayerMovement moveScript;
    private PlayerTargeting targetScript;
    private Camera cam;

    private float yaw = 0;
    private float pitch = 0;

    public float cameraSensitivityX = 5;
    public float cameraSensitivityY = 5;

    private float shakeIntensity = 0;

    // Start is called before the first frame update
    void Start()
    {
        targetScript = moveScript.GetComponent<PlayerTargeting>();
        cam = GetComponentInChildren<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (moveScript && !moveScript.isDead)
        {
            PlayerOrbitCamera();

            transform.position = moveScript.transform.position;

            // if aiming, set camera rotation to look at target
            RotateCamToLookAtTarget();

            // "zoom" in the camera
            ZoomCamera();

            // shake the camera
            ShakeCamera();
        }
    }

    public void Shake(float intensity = 1)
    {
        shakeIntensity = intensity;
    }

    private void ShakeCamera()
    {
        if (shakeIntensity < 0) shakeIntensity = 0;

        if (shakeIntensity > 0) shakeIntensity -= Time.deltaTime;
        else return;

        Quaternion targetRot = AnimMath.Lerp(UnityEngine.Random.rotation, Quaternion.identity, 0.99f);

        cam.transform.localRotation = AnimMath.Lerp(cam.transform.localRotation, cam.transform.localRotation * targetRot, shakeIntensity);
    }

    private bool IsTargeting()
    {
        return (targetScript && targetScript.target && targetScript.wantsToTarget);
    }

    private void ZoomCamera()
    {
        float dis = 10;

        if (IsTargeting())
        {
            dis = 5;
        }

        Vector3 targetPos = new Vector3(0, 0, -dis);

        cam.transform.localPosition = AnimMath.Slide(cam.transform.localPosition, targetPos, 0.001f);
    }

    private void RotateCamToLookAtTarget()
    {
        if (IsTargeting())
        {
            // if targeting, set rotation to look at target

            Vector3 vToTarget = targetScript.target.position - cam.transform.position;

            Quaternion targetRot = Quaternion.LookRotation(vToTarget, Vector3.up);

            cam.transform.rotation = AnimMath.Slide(cam.transform.rotation, targetRot, 0.001f);
        }
        else
        {
            // if NOT targeting, reset rotation

            Quaternion targetRot = Quaternion.identity; // no rotation...

            cam.transform.localRotation = AnimMath.Slide(cam.transform.localRotation, targetRot, 0.001f);
        }
    }

    private void PlayerOrbitCamera()
    {
        float mx = Input.GetAxisRaw("Mouse X");
        float my = Input.GetAxisRaw("Mouse Y");

        yaw += mx * cameraSensitivityX;
        pitch += my * cameraSensitivityY;

        if (IsTargeting())
        {
            pitch = Mathf.Clamp(pitch, 15, 60);
            // find player yaw
            float playerYaw = moveScript.transform.eulerAngles.y;
            // clamp camera-rig yaw to playerYaw +- 30
            yaw = Mathf.Clamp(yaw, playerYaw - 40, playerYaw + 40);
        }
        else
        {
            pitch = Mathf.Clamp(pitch, -10, 89);
        }

        transform.rotation = AnimMath.Slide(transform.rotation, Quaternion.Euler(pitch, yaw, 0), .001f);
    }
}
