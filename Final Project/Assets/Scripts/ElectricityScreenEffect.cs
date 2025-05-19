using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(RawImage))]
public class ElectricityScreenEffect : MonoBehaviour
{
    [Tooltip("Material that uses the electricity shader.")]
    public Material electricityMat;

    [Tooltip("Duration the effect should stay active.")]
    public float effectDuration = 1.5f;

    private RawImage rawImage;
    private bool isActive = false;

    private void Awake()
    {
        rawImage = GetComponent<RawImage>();

        if (electricityMat == null)
            Debug.LogWarning("Electricity material is not assigned.");

        // Assign material instance to avoid modifying shared material
        if (electricityMat != null)
            rawImage.material = Instantiate(electricityMat);

        rawImage.enabled = false; // start hidden
    }

    public void StartEffect()
    {
        if (rawImage == null || rawImage.material == null)
            return;

        isActive = true;
        rawImage.enabled = true;
        rawImage.material.SetFloat("_Speed", 1f); // example starting speed
        StartCoroutine(StopAfter(effectDuration));
    }

    private IEnumerator StopAfter(float duration)
    {
        yield return new WaitForSeconds(duration);
        isActive = false;
        if (rawImage != null)
            rawImage.enabled = false;
    }

    private void Update()
    {
        if (isActive && rawImage != null && rawImage.material != null)
        {
            // Animate speed or any other parameter over time
            float speed = Mathf.PingPong(Time.time, 1.5f) + 0.5f;
            rawImage.material.SetFloat("_Speed", speed);
        }
    }
}
