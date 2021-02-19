using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTargeting : MonoBehaviour
{

    public Transform target;
    public bool wantsToTarget = false;
    public float visionDis = 10;
    public float visionAngle = 90;

    private float searchCooldown = 0;
    private float pickCooldown = 0;

    private List<TargetableThing> potentaialTargets = new List<TargetableThing>();

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        wantsToTarget = Input.GetButton("Fire2");

        if (!wantsToTarget) target = null;

        searchCooldown -= Time.deltaTime; // counting down...
        if (searchCooldown <= 0 || (wantsToTarget && !target)) ScanForTargets(); // do this when countdown finished

        pickCooldown -= Time.deltaTime; // counting down...
        if (pickCooldown <= 0) PickATarget(); // do this when countdown finished

        if (target && !canSeeThing(target)) target = null;
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
