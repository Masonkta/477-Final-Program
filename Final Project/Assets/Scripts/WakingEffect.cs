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

    private float[] blinkDurations = { 0.8f, 0.6f }; 

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
        yield return new WaitForSeconds(1f); // initial pause before first blink

        int totalBlinks = blinkDurations.Length;

        while (currentBlink < totalBlinks)
        {
            float fadeOutDuration;
            float blinkPause;
            float fadeInDuration;

            if (currentBlink == 0)
            {
           
                fadeOutDuration = .5f;
                blinkPause = 1.0f;
                fadeInDuration = .5f;
            }
            else
            {

                fadeOutDuration = 0.3f;
                blinkPause = 0.4f;
                fadeInDuration = 0.3f;
            }

            yield return StartCoroutine(Fade(1, 0, fadeOutDuration));
            yield return new WaitForSeconds(blinkPause);

            float t = (currentBlink + 1) / (float)totalBlinks;
            float newXRotation = Mathf.Lerp(startXRotation, endXRotation, t);
            targetRotation = Quaternion.Euler(newXRotation, yRotation, 0f);

            yield return StartCoroutine(Fade(0, 1, fadeInDuration));
            yield return new WaitForSeconds(0.1f); // short pause between blinks

            currentBlink++;
        }

        // Final fade in (in case you want to emphasize waking up fully)
        yield return StartCoroutine(Fade(1, 0, 0.5f));

        targetRotation = Quaternion.Euler(endXRotation, yRotation, 0f);
        yield return new WaitForSeconds(0.1f); // allow smoothing

        fadeImage.gameObject.SetActive(false);
        uiclickerscript.enabled = true;
        headlockscript.enabled = true;
    }


    IEnumerator Fade(float startAlpha, float endAlpha, float duration)
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

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            color.a = Mathf.Lerp(startAlpha, endAlpha, t);
            fadeImage.color = color;
            vignette.intensity.value = Mathf.Lerp(0.6f, 0f, t);
            dof.gaussianStart.value = Mathf.Lerp(startGaussianStart, endGaussianStart, t);
            dof.gaussianEnd.value = Mathf.Lerp(startGaussianEnd, endGaussianEnd, t);
            yield return null;
        }

        color.a = endAlpha;
        fadeImage.color = color;
        vignette.intensity.value = endAlpha == 0 ? 0f : 0.6f;
        dof.gaussianStart.value = endGaussianStart;
        dof.gaussianEnd.value = endGaussianEnd;

        if (currentBlink >= blinkDurations.Length)
            dof.active = false;
    }
}
