using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class HeadLock : MonoBehaviour
{
    public float sensitivity = 2f;
    public Vector2 clampY = new Vector2(55f, 120f);    
    public Vector2 clampX = new Vector2(-17f, 43f);    
    private float rotationY = 0f; 
    private float rotationX = 0f; 

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        rotationY = transform.eulerAngles.y;
        rotationX = transform.eulerAngles.x;
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity;
        rotationY += mouseX;
        rotationX -= mouseY;
        rotationY = Mathf.Clamp(rotationY, clampY.x, clampY.y);
        rotationX = Mathf.Clamp(rotationX, clampX.x, clampX.y);
        transform.rotation = Quaternion.Euler(rotationX, rotationY, 0f);
    }
}
