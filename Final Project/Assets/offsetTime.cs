using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// Attach this to the object with the Animator
public class offsetTime : MonoBehaviour
{
    public int totalFrames = 300; // Total frames in the animation (e.g., 10s * 30fps)

    void Start()
    {
        Animator animator = GetComponent<Animator>();

        // Pick a random offset between 0 and totalFrames
        int frameOffset = Random.Range(0, totalFrames);
        float normalizedTime = (float)frameOffset / totalFrames;

        animator.Play("Dance", 0, normalizedTime);
    }
}
