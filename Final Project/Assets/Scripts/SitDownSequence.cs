using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class PlayerSitDown : MonoBehaviour
{
    public Camera playerCamera;
    public Camera player2Camera;
    public GameObject playerController;
    public MonoBehaviour playerMovementScript;
    public GameObject UiEventSystem;
    public GameObject CursorCanvas;
    public GameObject GameHandler;
    public GameObject ChairCanvas;
    public GameObject player2Model;  

    public float zoomDuration = 2.5f;
    public float rotationDuration = 2f;
    public float loweringDuration = 2f; 
    public float lookUpDuration = 2f; 
    public float zoomThreshold = 0.8f; 

    public Transform chairTargetPosition;

    private bool isNearChair = false;
    public bool isSitting = false;

    private Vector3 player2OriginalPosition;
    private Quaternion player2OriginalRotation;
    private Vector3 player2CameraOriginalLocalPos;
    private Quaternion player2CameraOriginalLocalRot;

    public GameObject Canvastv1;
    public GameObject Canvastv2;
    public GameObject tv1;
    public GameObject tv2;
    public Material originalMaterialTV1;
    public Material originalMaterialTV2;

    void Start()
    {
        ChairCanvas.SetActive(false);
        player2OriginalPosition = playerController.transform.position;
        player2OriginalRotation = playerController.transform.rotation;
        player2CameraOriginalLocalPos = player2Camera.transform.localPosition;
        player2CameraOriginalLocalRot = player2Camera.transform.localRotation;
    }

    void Update()
    {
        if (isNearChair && !isSitting)
        {
            ChairCanvas.SetActive(true);
            if (Input.GetKeyDown(KeyCode.E))
            {
                StartCoroutine(SitSequence());
            }
        }
    }

    IEnumerator SitSequence()
    {
        isSitting = true;
        ChairCanvas.SetActive(false);
        playerMovementScript.enabled = false;

        Vector3 camStartPos = player2Camera.transform.localPosition;
        Vector3 camEndPos = new Vector3(0, 1.6f, 0);
        Quaternion camStartRot = player2Camera.transform.localRotation;
        Quaternion camEndRot = Quaternion.identity;

        float camProgress = 0f;
        while (camProgress < 1f)
        {
            camProgress += Time.deltaTime / zoomDuration;
            player2Camera.transform.localPosition = Vector3.Lerp(camStartPos, camEndPos, camProgress);
            player2Camera.transform.localRotation = Quaternion.Slerp(camStartRot, camEndRot, camProgress);

            if (camProgress >= zoomThreshold && player2Model.activeSelf)
            {
                player2Model.SetActive(false);
            }

            yield return null;
        }

        Vector3 seatedCamWorldPos = player2Camera.transform.position;
        Quaternion seatedCamWorldRot = player2Camera.transform.rotation;

        playerController.SetActive(false);
        playerCamera.transform.position = seatedCamWorldPos;
        playerCamera.transform.rotation = seatedCamWorldRot;
        playerCamera.gameObject.SetActive(true);
        GameHandler.SetActive(false);

        Quaternion startRot = playerCamera.transform.rotation;
        Quaternion lowerRot = Quaternion.Euler(50f, 87.84f, 0f);
        Vector3 finalPos = new Vector3(0, 0f, 0);
        Vector3 startPos = playerCamera.transform.localPosition;

        float lowerProgress = 0f;
        while (lowerProgress < 1f)
        {
            lowerProgress += Time.deltaTime / loweringDuration;
            playerCamera.transform.localPosition = Vector3.Lerp(startPos, finalPos, lowerProgress);
            playerCamera.transform.rotation = Quaternion.Slerp(startRot, lowerRot, lowerProgress);

            yield return null;
        }

        playerCamera.transform.localPosition = finalPos;
        playerCamera.transform.rotation = lowerRot;

        Quaternion lookUpRot = Quaternion.Euler(-9.7f, 87.84f, 0f);
        float lookUpProgress = 0f;
        while (lookUpProgress < 1f)
        {
            lookUpProgress += Time.deltaTime / lookUpDuration;
            playerCamera.transform.rotation = Quaternion.Slerp(lowerRot, lookUpRot, lookUpProgress);

            yield return null;
        }
        playerCamera.transform.rotation = lookUpRot;

        // Enable components and reset transforms
        if (playerCamera.TryGetComponent(out HeadLock headLockScript)) headLockScript.enabled = true;
        if (playerCamera.TryGetComponent(out UIClicker uiClickerScript)) uiClickerScript.enabled = true;

        CursorCanvas.SetActive(true);
        UiEventSystem.SetActive(true);

        playerController.transform.position = player2OriginalPosition;
        playerController.transform.rotation = player2OriginalRotation;
        player2Camera.transform.localPosition = player2CameraOriginalLocalPos;
        player2Camera.transform.localRotation = player2CameraOriginalLocalRot;

        Canvastv1.SetActive(true);
        Canvastv2.SetActive(true);
        tv1.GetComponent<MeshRenderer>().material = originalMaterialTV1;
        tv2.GetComponent<MeshRenderer>().material = originalMaterialTV2;

        Debug.Log("Seated.");
    }


    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isNearChair = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isNearChair = false;
            ChairCanvas.SetActive(false);
        }
    }
}
