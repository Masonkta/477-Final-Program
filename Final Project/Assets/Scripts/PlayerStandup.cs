using UnityEngine;
using UnityEngine.UI;

public class PlayerStandup : MonoBehaviour
{
    public Camera playerCamera;              
    public GameObject playerController;      
    public Button standUpButton;              

    public float rotationSpeed = 2f;
    public float riseSpeed = 2f;
    public float targetHeightOffset = 1.5f;

    private bool isRotating = false;
    private bool isRising = false;

    private Quaternion targetRotation;
    private Vector3 startPosition;
    private Vector3 targetPosition;

    public GameObject UiEventSystem;
    public GameObject CursorCanvas;
    public GameObject GameHandler;

    void Start()
    {
        // Attach button press to function
        standUpButton.onClick.AddListener(StartSequence);

        // Calculate target rotation (looking straight ahead)
        targetRotation = Quaternion.Euler(0, 88, 0);

        // Save the starting seated position of the camera
        startPosition = playerCamera.transform.position;

        // Calculate standing position
        targetPosition = startPosition + new Vector3(0, targetHeightOffset, 0);

        playerController.SetActive(false);
    }

    public void StartSequence()
    {
        Debug.Log("Stand");
        if (isRotating || isRising) return;
        // Disable the HeadLock script
        HeadLock headLockScript = playerCamera.GetComponent<HeadLock>();
        if (headLockScript != null)
        {
            headLockScript.enabled = false;
        }

        // Disable the UIClicker script
        UIClicker uiClickerScript = playerCamera.GetComponent<UIClicker>();
        if (uiClickerScript != null)
        {
            uiClickerScript.enabled = false;
        }
        standUpButton.image.color = new Color(1, 1, 1, 0f);
        isRotating = true;
    }

    void Update()
    {
        if (isRotating)
        {
            // Rotate towards forward smoothly
            playerCamera.transform.rotation = Quaternion.Slerp(
                playerCamera.transform.rotation,
                targetRotation,
                Time.deltaTime * rotationSpeed
            );

            if (Quaternion.Angle(playerCamera.transform.rotation, targetRotation) < 0.5f)
            {
                playerCamera.transform.rotation = targetRotation;
                isRotating = false;
                isRising = true;
            }
        }
        else if (isRising)
        {
            // Move up smoothly to standing height
            playerCamera.transform.position = Vector3.MoveTowards(
                playerCamera.transform.position,
                targetPosition,
                riseSpeed * Time.deltaTime
            );

            if (Vector3.Distance(playerCamera.transform.position, targetPosition) < 0.05f)
            {
                playerCamera.transform.position = targetPosition;
                isRising = false;

                CompleteTransition();
            }
        }
    }

    void CompleteTransition()
    {
        playerCamera.gameObject.SetActive(false);
        playerController.SetActive(true);
        CursorCanvas.SetActive(false);
        UiEventSystem.SetActive(false);
        GameHandler.SetActive(true);
    }
}
