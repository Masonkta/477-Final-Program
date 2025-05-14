using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadLock : MonoBehaviour
{
    // Sensitivity
    public float sensitivity = 2f;

    // X and Y Constraints
    public Vector2 clampY = new Vector2(55f, 120f);
    public Vector2 clampX = new Vector2(-17f, 43f);

    public float rotationY = 0f;
    public float rotationX = 0f;
    private float mouseX = 0f;
    private float mouseY = 0f;
    public WakingEffect WakingupScript;
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        if (Camera.main != null)
        {
            rotationX = -9.7f;
            rotationY = 88f;
            Camera.main.transform.localPosition = Vector3.zero;
            Camera.main.transform.localRotation = Quaternion.Euler(-9.7f, 88f, 0f);
            mouseX = rotationY / sensitivity;
            mouseY = -rotationX / sensitivity;
        }
    }

    void OnEnable()
    {
        if (Camera.main != null)
        {
            rotationX = -9.7f;
            rotationY = 88f;
            Camera.main.transform.localRotation = Quaternion.Euler(-9.7f, 88f, 0f);
            mouseX = rotationY / sensitivity;
            mouseY = -rotationX / sensitivity;
        }
        WakingupScript.enabled = false;
    }

    void Update()
    {
        // Capture input
        mouseX = Input.GetAxis("Mouse X") * sensitivity;
        mouseY = Input.GetAxis("Mouse Y") * sensitivity;

        // Apply rotation
        rotationY += mouseX;
        rotationX -= mouseY;

        // Clamp rotation
        rotationY = Mathf.Clamp(rotationY, clampY.x, clampY.y);
        rotationX = Mathf.Clamp(rotationX, clampX.x, clampX.y);

        // Apply to transform
        transform.rotation = Quaternion.Euler(rotationX, rotationY, 0f);
    }
}
