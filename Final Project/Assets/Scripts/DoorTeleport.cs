using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DoorTeleport : MonoBehaviour
{
    [Header("Teleport Settings")]
    public Transform targetSpawnPoint;
    public GameObject roomToEnable;
    public GameObject roomToDisable;

    [Header("Fade Settings")]
    public Canvas fadeCanvas;
    public Image fadeImage;
    public float fadeDuration = 1f;

    [Header("Interaction")]
    public KeyCode interactKey = KeyCode.E;

    public GameObject player;
    public CharacterController characterController;
    private bool canTeleport = false;
    public GameObject interactionCanvas;

    private void Start()
    {


        if (fadeCanvas != null)
        {
            fadeCanvas.enabled = true;
            SetFadeAlpha(0f);
            Debug.Log("Fade canvas found and initialized.");
        }
        else
        {
            Debug.LogWarning("Fade canvas not assigned.");
        }

        if (fadeImage == null)
        {
            Debug.LogWarning("Fade image not assigned.");
        }
    }

    private void Update()
    {
        if (canTeleport && Input.GetKeyDown(interactKey))
        {
            Debug.Log("Interact key pressed. Starting teleport.");
            StartCoroutine(HandleTeleport());
        }
    }

    private IEnumerator HandleTeleport()
    {
        canTeleport = false;
        interactionCanvas.SetActive(false);

        if (characterController != null)
        {
            characterController.enabled = false;
            Debug.Log("CharacterController disabled.");
        }

        yield return StartCoroutine(Fade(0f, 1f));
        Debug.Log("Fade to black complete.");

        if (targetSpawnPoint != null && player != null)
        {
            player.transform.position = targetSpawnPoint.position;
            player.transform.rotation = targetSpawnPoint.rotation;
            Debug.Log($"Player moved to target spawn point at {targetSpawnPoint.position}.");
        }
        else
        {
            Debug.LogError("Player or target spawn point is null during teleport.");
        }

        if (roomToDisable != null)
        {
            roomToDisable.SetActive(false);
            Debug.Log("Disabled room: " + roomToDisable.name);
        }

        if (roomToEnable != null)
        {
            roomToEnable.SetActive(true);
            Debug.Log("Enabled room: " + roomToEnable.name);
        }

        yield return StartCoroutine(Fade(1f, 0f));
        Debug.Log("Fade back in complete.");

        if (characterController != null)
        {
            characterController.enabled = true;
            Debug.Log("CharacterController re-enabled.");
        }
    }

    private IEnumerator Fade(float from, float to)
    {
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(from, to, t / fadeDuration);
            SetFadeAlpha(alpha);
            yield return null;
        }
        SetFadeAlpha(to);
    }

    private void SetFadeAlpha(float alpha)
    {
        if (fadeImage != null)
        {
            Color c = fadeImage.color;
            c.a = alpha;
            fadeImage.color = c;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canTeleport = true;
            Debug.Log("Player entered trigger zone.");
            interactionCanvas.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canTeleport = false;
            Debug.Log("Player exited trigger zone.");
            interactionCanvas.SetActive(false);
        }
    }
}
