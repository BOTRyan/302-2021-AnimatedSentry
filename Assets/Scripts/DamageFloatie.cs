using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageFloatie : MonoBehaviour
{

    private GameObject player;
    private TMPro.TextMeshPro textInfo;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerMovement>().gameObject;
        textInfo = GetComponent<TMPro.TextMeshPro>();
    }

    // Update is called once per frame
    void Update()
    {
        if (player)
        {
            Camera cam = Camera.main;

            Vector3 disToTarget = cam.transform.position - transform.position;

            Quaternion targetRotation = Quaternion.LookRotation(disToTarget, Vector3.up);

            Vector3 euler1 = transform.localEulerAngles; // get local angles BEFORE rotation
            Quaternion prevRot = transform.rotation;
            transform.rotation = targetRotation; // set rotation 
            Vector3 euler2 = transform.localEulerAngles; // get local angles AFTER rotation

            euler2.y += 180;

            transform.rotation = prevRot; // revert rotation

            // animate rotation
            transform.localRotation = Quaternion.Euler(euler2);

            textInfo.alpha -= Time.deltaTime;
            transform.localPosition = AnimMath.Slide(transform.localPosition, new Vector3(transform.localPosition.x, transform.localPosition.y + 1, transform.localPosition.z), 0.1f);

            if (textInfo.alpha <= 0) Destroy(gameObject);
        }
    }
}
