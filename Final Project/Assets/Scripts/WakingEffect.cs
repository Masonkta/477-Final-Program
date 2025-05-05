using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class WakingEffect : MonoBehaviour
{
    public Image fadeImage;
    public AudioSource heartbeatAudio;
    public Volume postProcessVolume;

    private float[] blinkDurations = { 0.6f, 0.8f }; 
    private float fadeDuration = 0.3f;

    private Vignette vignette;
    private DepthOfField dof;
    private int currentBlink = 0;
    private Quaternion targetRotation;
    private Quaternion startRotation;

    private float startXRotation = 50f;
    private float endXRotation = -10f;
    private float yRotation = 88f;

    public UIClicker uiclickerscript;
    public HeadLock headlockscript;



    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        uiclickerscript.enabled = false;
        headlockscript.enabled = false;
        postProcessVolume.profile.TryGet(out vignette);
        postProcessVolume.profile.TryGet(out dof);
        vignette.intensity.value = 0.6f;
        dof.focusDistance.value = 1.0f;
        startRotation = Quaternion.Euler(startXRotation, yRotation, 0f);
        Camera.main.transform.localRotation = startRotation;
        targetRotation = startRotation;
        postProcessVolume.profile.TryGet(out dof);
        dof.gaussianStart.value = 0.1f;     
        dof.gaussianEnd.value = 1f;            
        StartCoroutine(BlinkSequence());
    }

    private void Update()
    {
        Camera.main.transform.localRotation = Quaternion.Slerp(Camera.main.transform.localRotation, targetRotation, Time.deltaTime * 1.5f);
    }



    IEnumerator BlinkSequence()
    {
        yield return new WaitForSeconds(1f); //initial pause before first blink
                                             //heartbeatAudio.Play();
        while (currentBlink < blinkDurations.Length)
        {
            yield return StartCoroutine(Fade(1, 0));
            yield return new WaitForSeconds(blinkDurations[currentBlink]);

            float t = (currentBlink + 1) / (float)blinkDurations.Length;
            float newXRotation = Mathf.Lerp(startXRotation, endXRotation, t);
            targetRotation = Quaternion.Euler(newXRotation, yRotation, 0f);

            yield return StartCoroutine(Fade(0, 1));
            yield return new WaitForSeconds(0.4f); // short pause between blinks

            currentBlink++;
        }

        // Final fade in
        yield return StartCoroutine(Fade(1, 0));

        targetRotation = Quaternion.Euler(endXRotation, yRotation, 0f);
        yield return new WaitForSeconds(0.1f); // allow a few frames to smooth rotation

        yield return StartCoroutine(Fade(1, 0));
        fadeImage.gameObject.SetActive(false);

        //heartbeatAudio.Stop();
        uiclickerscript.enabled = true;
        headlockscript.enabled = true;
    }


    IEnumerator Fade(float startAlpha, float endAlpha)
    {
        float elapsed = 0f;
        Color color = fadeImage.color;
        float startGaussianStart = dof.gaussianStart.value;
        float startGaussianEnd = dof.gaussianEnd.value;
        float endGaussianStart = startGaussianStart;
        float endGaussianEnd = startGaussianEnd;

        if (currentBlink >= 1 && endAlpha == 0)
        {
            endGaussianStart = 50f;
            endGaussianEnd = 100f;
        }

        // Fade timing
        float fadeTimeMultiplier = 1.0f;

        while (elapsed < fadeDuration * fadeTimeMultiplier)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / (fadeDuration * fadeTimeMultiplier);
            // Fade image alpha
            color.a = Mathf.Lerp(startAlpha, endAlpha, t);
            fadeImage.color = color;
            vignette.intensity.value = Mathf.Lerp(0.6f, 0f, t);
            dof.gaussianStart.value = Mathf.Lerp(startGaussianStart, endGaussianStart, t);
            dof.gaussianEnd.value = Mathf.Lerp(startGaussianEnd, endGaussianEnd, t);
            yield return null;
        }

        // Snap to final values
        color.a = endAlpha;
        fadeImage.color = color;
        vignette.intensity.value = endAlpha == 0 ? 0f : 0.6f;
        dof.gaussianStart.value = endGaussianStart;
        dof.gaussianEnd.value = endGaussianEnd;

        // Disable DoF completely after final blink
        if (currentBlink >= blinkDurations.Length)
        {
            dof.active = false;
        }
    }
}
