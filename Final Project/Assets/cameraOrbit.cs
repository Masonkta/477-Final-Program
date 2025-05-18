using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    public float xCamOffSet; bool startedShift = false;

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

        heightAbove += Time.deltaTime / 9f; heightAbove = Mathf.Clamp(heightAbove, 0f, 2f);

        camTransform.localPosition = new Vector3(Mathf.Sin(angle) * dist, heightAbove, Mathf.Cos(angle) * dist);

        lookAtZ -= Time.deltaTime * 0.7f; if (lookAtZ < 0f) lookAtZ = 0f;
        lookAtTransform.localPosition = new Vector3(0f, 0f, lookAtZ);
        camTransform.LookAt(lookAtTransform);

        // if (Time.timeSinceLevelLoad > 5f)
        if (Time.timeSinceLevelLoad > 7.5f && !startedShift)
            StartCoroutine(shiftCamera());
    }

    IEnumerator shiftCamera()
    {
        startedShift = true;
        float duration = 4f;
        float elapsed = 0f;

        float startDist = dist;
        float endDist = startDist + 3f;

        float startHeight = heightAbove;
        float endHeight = startHeight + 1.2f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            dist = SmoothMap(t, 0f, 1f, startDist, endDist);
            heightAbove = SmoothMap(t, 0f, 1f, startHeight, endHeight);

            yield return null;
        }

        // Ensure final values are accurate
        dist = endDist;
        heightAbove = endHeight;
    }

    float Map(float value, float start1, float end1, float start2, float end2)
    {
        return (value - start1) / (end1 - start1) * (end2 - start2) + start2;
    }

    float SmoothMap(float value, float start1, float end1, float start2, float end2)
    {
        float t = (value - start1) / (end1 - start1);               // Normalize to [0, 1]
        t = t * t * (3f - 2f * t);                                  // Apply SmoothStep
        return (1 - t) * start2 + t * end2;                         // Interpolate using smoothed t
    }


}
