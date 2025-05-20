using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
public class UIClicker : MonoBehaviour
{
    public Camera playerCamera;
    private Button currentlyHoveredButton;
    public Animator m_Animator;
    public DDOL godScript;
    public GameObject optionsCanvas;
    public HeadLock headLockScript;
    private bool optionsOpen = false;
    public Vector3 lookTarget;
    public ElectricityScreenEffect screenEffect;
    public GameObject electricityParticles;
    public Image fadeOverlay;
    public AudioSource AudioSource;
    public AudioClip SoundClip;

    void Start()
    {
        godScript = GameObject.Find("DDOL").GetComponent<DDOL>();
        lookTarget = GameObject.Find("Ch44_nonPBR@Orc Idle 1").transform.position;
    }

    void Update()
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = new Vector2(Screen.width / 2, Screen.height / 2)
        };

        var raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, raycastResults);
        Button hoveredButton = null;

        foreach (RaycastResult result in raycastResults)
        {
            if (result.gameObject.CompareTag("UIButton"))
            {
                hoveredButton = result.gameObject.GetComponent<Button>();

                if (Input.GetMouseButtonDown(0))
                {
                    hoveredButton.onClick.Invoke();
                }
                break; 
            }
        }

        if (hoveredButton != currentlyHoveredButton)
        {
            if (currentlyHoveredButton != null)
            {
                SetButtonAlpha(currentlyHoveredButton, 0f); 
            }
            if (hoveredButton != null)
            {
                SetButtonAlpha(hoveredButton, 0.2f);
            }
            currentlyHoveredButton = hoveredButton;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (optionsOpen)
            {
                CloseOptions();
            }
        }

}

    void SetButtonAlpha(Button button, float alpha)
    {
        Image image = button.GetComponent<Image>();
        if (image != null)
        {
            Color c = image.color;
            c.a = alpha;
            image.color = c;
        }
    }




    public void StartGame()
    {
        if (godScript.highestTaskAchieved == GameState.WOKE_UP)
            godScript.highestTaskAchieved = GameState.ENTERED_CITY;

        Debug.Log("START");
        StartCoroutine(PlaySaluteAndLoadScene());
    }

    private IEnumerator PlaySaluteAndLoadScene()
    {
        // Disable headlock so we can control the camera
        if (headLockScript != null)
            headLockScript.enabled = false;

        // Smoothly rotate camera to look at the captain
        if (playerCamera != null && lookTarget != null)
            yield return StartCoroutine(SmoothLookAt(playerCamera.transform, lookTarget, 1.5f));

        // Play salute animation
        if (m_Animator != null)
            m_Animator.SetTrigger("Salute");

        AudioSource.clip = SoundClip;
        AudioSource.volume = 0.7f;
        AudioSource.Play();
        electricityParticles.SetActive(true);
        screenEffect.StartEffect();

        yield return new WaitForSeconds(1f); 

        yield return StartCoroutine(TiltCameraDown(playerCamera.transform, 70f, 2f));


        yield return StartCoroutine(FadeToBlack(2f));

        // Load city scene
        SceneManager.LoadScene("City Scene");
    }

    private IEnumerator SmoothLookAt(Transform cameraTransform, Vector3 targetPosition, float duration)
    {
        Quaternion startRot = cameraTransform.rotation;
        Quaternion targetRot = Quaternion.LookRotation(targetPosition - cameraTransform.position);
        float elapsed = 0f;

        while (elapsed < duration)
        {
            cameraTransform.rotation = Quaternion.Slerp(startRot, targetRot, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        cameraTransform.rotation = targetRot; // Snap to final rotation
    }

  

    private IEnumerator FadeToBlack(float duration)
    {
        Color c = fadeOverlay.color;
        float startAlpha = 0f;
        float endAlpha = 1f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            c.a = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
            fadeOverlay.color = c;
            elapsed += Time.deltaTime;
            yield return null;
        }

        c.a = endAlpha;
        fadeOverlay.color = c;
    }


    private IEnumerator TiltCameraDown(Transform cam, float angle, float duration)
    {
        Quaternion startRot = cam.rotation;
        Quaternion endRot = Quaternion.Euler(cam.eulerAngles.x + angle, cam.eulerAngles.y, cam.eulerAngles.z);
        float elapsed = 0f;

        while (elapsed < duration)
        {
            cam.rotation = Quaternion.Slerp(startRot, endRot, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        cam.rotation = endRot;
    }


    public void OpenOptions()
    {
        Debug.Log("Opening Options Menu");

        optionsCanvas.SetActive(true);
        headLockScript.inputEnabled = false;
        optionsOpen = true;
    }

    public void CloseOptions()
    {

        optionsCanvas.SetActive(false);  
        headLockScript.inputEnabled = true;
        optionsOpen = false;
    }
    public void QuitGame()
    {
        Application.Quit();

        #if UNITY_EDITOR
        EditorApplication.isPlaying = false;
        #endif
    }
}
