using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTargeting : MonoBehaviour
{

    public Transform target;
    public bool wantsToTarget = false;
    public bool wantsToAttack = false;
    public float visionDis = 10;
    public float visionAngle = 90;

    private float searchCooldown = 0;
    private float pickCooldown = 0;
    private float shootCooldown = 0;

    public float roundsPerSecond = 10;

    private List<TargetableThing> potentaialTargets = new List<TargetableThing>();

    public Transform armL;
    public Transform armR;

    private Vector3 startPosArmL;
    private Vector3 startPosArmR;

    public ParticleSystem prefabMuzzleFlash;
    public Transform handR;
    public Transform handL;

    CameraOrbit camOrbit;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        startPosArmL = armL.localPosition;
        startPosArmR = armR.localPosition;

        camOrbit = Camera.main.GetComponentInParent<CameraOrbit>();
    }

    // Update is called once per frame
    void Update()
    {
        wantsToTarget = Input.GetButton("Fire2");
        wantsToAttack = Input.GetButton("Fire1");

        if (!wantsToTarget) target = null;

        searchCooldown -= Time.deltaTime; // counting down...
        if (searchCooldown <= 0 || (wantsToTarget && !target)) ScanForTargets(); // do this when countdown finished

        pickCooldown -= Time.deltaTime; // counting down...
        if (pickCooldown <= 0) PickATarget(); // do this when countdown finished

        if (target && !canSeeThing(target)) target = null;

        if (shootCooldown > 0) shootCooldown -= Time.deltaTime;

        SlideArmsHome();

        DoAttack();
    }

    private void SlideArmsHome()
    {
        armL.localPosition = AnimMath.Slide(armL.localPosition, startPosArmL, 0.01f);
        armR.localPosition = AnimMath.Slide(armR.localPosition, startPosArmR, 0.01f);
    }

    private void DoAttack()
    {
        if (shootCooldown > 0) return;
        if (!wantsToTarget) return;
        if (!wantsToAttack) return;
        if (target == null) return;
        if (!canSeeThing(target)) return;

        HealthSystem targetHealth = target.GetComponent<HealthSystem>();
        if (targetHealth)
        {
            targetHealth.TakeDamage(20);
        }

        // resets cooldown:
        shootCooldown = 1 / roundsPerSecond;

        camOrbit.Shake(.25f);
        

        if (handL) Instantiate(prefabMuzzleFlash, handL.position, handL.rotation);
        if (handR) Instantiate(prefabMuzzleFlash, handR.position, handR.rotation);

        // rotates the arm up:
        armL.localEulerAngles += new Vector3(-20, 0, 0);
        armR.localEulerAngles += new Vector3(-20, 0, 0);

        // moves the arms backward:
        armL.position += -armL.forward * 0.1f;
        armR.position += -armR.forward * 0.1f;
    }

    private bool canSeeThing(Transform thing)
    {
        if (!thing) return false; // uh... error

        Vector3 vToThing = thing.position - transform.position; // 

        // check distance:
        if (vToThing.sqrMagnitude > visionDis * visionDis) return false; // too far to see

        // check direction:
        if (Vector3.Angle(transform.forward, vToThing) > visionAngle) return false; // out of vision cone

        // TODO: check occlusion

        return true;
    }

    void ScanForTargets()
    {
        // do the next scan in 1 seconds
        searchCooldown = 1;

        // empty the list
        potentaialTargets.Clear();

        // refill the list:
        TargetableThing[] things = GameObject.FindObjectsOfType<TargetableThing>();


        foreach (TargetableThing thing in things)
        {
            if (canSeeThing(thing.transform))
            {
                potentaialTargets.Add(thing);
            }
        }
    }

    void PickATarget()
    {
        pickCooldown = .25f;

        target = null; // clear target, and get a new one

        float closestDistSoFar = 0;

        // find closest targetable thing and sets it as our target:
        foreach (TargetableThing pt in potentaialTargets)
        {
            float dd = (pt.transform.position - transform.position).sqrMagnitude;

            if (dd < closestDistSoFar || target == null)
            {
                target = pt.transform;
                closestDistSoFar = dd;
            }
        }
    }
}
