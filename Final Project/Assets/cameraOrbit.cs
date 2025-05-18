using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraOrbit : MonoBehaviour
{
    public float angle = 0f;
    public float dist = 0f;
    public float D_dist;
    public float heightAbove = 0f;
    public Transform camTransform;
    public Transform lookAtTransform;
    public float lookAtZ;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        angle += Time.deltaTime * Mathf.PI * 2f / 6f; // Take 6 seconds to revolve

        D_dist -= Time.deltaTime / 20f; if (D_dist < 0f) D_dist = 0f;
        dist += D_dist * Time.deltaTime;

        heightAbove += Time.deltaTime / 14f; heightAbove = Mathf.Clamp(heightAbove, 0f, 1.6f);
        
        camTransform.localPosition = new Vector3(Mathf.Sin(angle) * dist, heightAbove, Mathf.Cos(angle) * dist);

        lookAtZ -= Time.deltaTime * 0.7f; if (lookAtZ < 0f) lookAtZ = 0f;
        lookAtTransform.localPosition = new Vector3(0f, 0f, lookAtZ);
        camTransform.LookAt(lookAtTransform);
    }
}
