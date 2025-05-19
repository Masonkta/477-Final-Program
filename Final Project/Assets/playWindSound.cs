using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class playWindSound : MonoBehaviour
{
    public AudioSource windSource;
    public float noiseScale;
    float x;
    // Start is called before the first frame update
    void Start()
    {
        x = Random.Range(-15f, 15f);
    }

    // Update is called once per frame
    void Update()
    {
        x += Time.deltaTime;
        float f1 = 0.3f * Mathf.Sin(0.3f * x - 1.5f) + 0.4f;
        float f2 = -0.2f * Mathf.Sin(0.7f * x + 3f);
        float f3 = -0.3f * Mathf.Sin(0.06f * x + 0.3f);
        noiseScale = f1 + f2 + f3;
        noiseScale = Mathf.Clamp(noiseScale, 0f, 1f);
        windSource.volume = noiseScale * 0.8f;

        float p1 = 0.22f * Mathf.Sin(0.3f * x) + 0.9f;
        windSource.pitch = Mathf.Clamp(p1, 0f, 2f);
    }
}
