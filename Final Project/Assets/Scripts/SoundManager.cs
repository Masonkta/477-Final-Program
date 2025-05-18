using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip introClip;   // Plays once
    public AudioClip loopClip;    // Loops afterward

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        StartCoroutine(PlayIntroThenLoop());
    }

    IEnumerator PlayIntroThenLoop()
    {
        // Play the intro clip once
        audioSource.clip = introClip;
        audioSource.loop = false;
        audioSource.volume = 0.3f;
        audioSource.Play();

        // Wait until it's done playing
        yield return new WaitForSeconds(7.5f);

        // Play the looping clip
        audioSource.clip = loopClip;
        audioSource.loop = true;
        audioSource.Play();
    }
}
