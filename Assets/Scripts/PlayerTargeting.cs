using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTargeting : MonoBehaviour
{

    public Transform target;
    public bool wantsToTarget = false;
    public float visionDis = 10;

    private float searchCooldown = 0;
    private float pickCooldown = 0;

    private List<TargetableThing> potentaialTargets = new List<TargetableThing>();

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        wantsToTarget = Input.GetButton("Fire2");

        searchCooldown -= Time.deltaTime; // counting down...
        if (searchCooldown <= 0) ScanForTargets(); // do this when countdown finished

        pickCooldown -= Time.deltaTime; // counting down...
        if (pickCooldown <= 0) PickATarget(); // do this when countdown finished
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
            // check how far away thing is
            Vector3 disToThing = thing.transform.position - transform.position;

            if (disToThing.sqrMagnitude < visionDis * visionDis)
            {
                if (Vector3.Angle(transform.forward, disToThing) < 45)
                {
                    potentaialTargets.Add(thing);
                }

            }

            // check what direction it is in
        }
    }

    void PickATarget()
    {
        pickCooldown = .25f;

        if (target) return; // we already have a target...

        float closestDistSoFar = 0;

        // find closest targetable thing and sets it as our target:
        foreach(TargetableThing pt in potentaialTargets)
        {
            float dd = (pt.transform.position - transform.position).sqrMagnitude;

            if(dd < closestDistSoFar || target == null)
            {
                target = pt.transform;
                closestDistSoFar = dd;
            }
        }
    }
}
