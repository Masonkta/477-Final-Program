using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class playerMovement : MonoBehaviour
{
    public Transform forwardTransform;
    public Transform cameraTransform;
    public Transform PovCameraTransform;
    public Transform lookAt;
    public bool canMove = true;
    public Animator playerAnimator;
    public bool isFirstPerson;

    [Header("Moving")]
    public bool isGrounded;
    public float walkSpeed = 8f;
    public float runSpeed = 18f;
    float moveSpeed;
    public float actualMoveSpeed;
    public float relativeSpeed;
    Vector3 movement;
    public Vector3 playerVelocity;
    public float groundCheckDistance = 1.08f;
    public float jumpHeight = 4f;
    bool sprinting;
    CharacterController controller;

    [Header("Turning")]
    public bool turningEnabled = true;
    public float xSens = 5f;
    public float ySens = 3f;
    public float ySensPOV = 3f;
    public Vector2 mouseMovement;
    float xRotationCam;

    [Header("Camera")]
    public float distMult = 1f;
    float distMultGoal = 1f;

    float camHeight = 2f;
    public float actualCamHeight;

    float camDist = 8f;
    public float actualCamDist;
    public bool hasKey = false;
    public DDOL DDOL;

    void Start()
    {
        assignPlayerComponents();
        isFirstPerson = PovCameraTransform;
        DDOL = GameObject.Find("DDOL").GetComponent<DDOL>();
    }

    void assignPlayerComponents()
    {
        controller = GetComponent<CharacterController>();

        if (transform.Find("Look Transform"))
            forwardTransform = transform.Find("Look Transform");

        if (transform.Find("Player Camera"))
            cameraTransform = transform.Find("Player Camera");

        if (transform.Find("POV"))
            PovCameraTransform = transform.Find("POV");

        if (transform.Find("Look At"))
            lookAt = transform.Find("Look At");

        playerAnimator = GetComponent<Animator>();
    }

    void Update()
    {
        if (Time.timeSinceLevelLoad > 0.5f)
        {
            handleInput();
            moving();
            
            if (GetComponent<NPCManager>())
            {
                if (!GetComponent<NPCManager>().freezeCamera)
                    turning();
            }
            else
            {
                turning();
            }
        }

        if (GetComponent<NPCManager>())
        {
            if (!GetComponent<NPCManager>().freezeCamera)
                handleCamera();
        }
        else
        {
            handleCamera();
        }
    }

    void handleInput()
    {
        sprinting = Input.GetKey(KeyCode.LeftShift);

        if (Input.GetKeyDown(KeyCode.Space))
            jumpOrDash();

        distMultGoal -= Input.mouseScrollDelta.y / 25f;
        distMultGoal = Mathf.Clamp(distMultGoal, 0.2f, 1.5f);
    }

    void moving()
    {
        moveSpeed = sprinting ? runSpeed : walkSpeed;
        movement = forwardTransform.TransformDirection(new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical")));
        if (movement.magnitude == 0) moveSpeed = 0f;

        float smoothFactor = moveSpeed > 0f ? 2f : 5f; // Adjust this value to control the smoothing speed
        actualMoveSpeed += (moveSpeed - actualMoveSpeed) * (1 - Mathf.Exp(-smoothFactor * Time.deltaTime));

        playerAnimator.SetFloat("Speed", actualMoveSpeed / runSpeed);
        relativeSpeed = actualMoveSpeed / runSpeed;
        playerAnimator.speed = relativeSpeed >= 0.235f ? (1.3f/(6.6f*relativeSpeed-0.6f)+1.3f) : (-(1.1f / (0.5f * relativeSpeed + 0.3f)) + 5.3f);
        print(relativeSpeed > 0.235f ? (1.3f / (6.6f * relativeSpeed - 0.6f) + 1.3f) : -(1.1f / (0.5f * relativeSpeed + 0.3f) + 5.3f));
        // playerAnimator.speed = 1.5f / (8.9f * actualMoveSpeed / runSpeed + 0.9f) + 1.3f;

        if (canMove)
            controller.Move(movement * actualMoveSpeed * Time.deltaTime);

        if (!isGrounded) playerVelocity.y += Time.deltaTime * Physics.gravity.y;
        controller.Move(playerVelocity * Time.deltaTime);

        if (LayerMask.NameToLayer("Ground") == -1)
        {
            isGrounded = controller.isGrounded || Physics.Raycast(transform.position, Vector3.down, groundCheckDistance);
        }
        else
        {
            int groundLayer = LayerMask.NameToLayer("Ground");
            isGrounded = controller.isGrounded || Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, 1 << groundLayer);
        }

        if (isGrounded)
        {
            playerVelocity = Vector3.down * Mathf.Max(3f, -playerVelocity.y);
        }
    }

    float Map(float value, float start1, float end1, float start2, float end2)
    {
        return (value - start1) / (end1 - start1) * (end2 - start2) + start2;
    }

    void turning()
    {
        turningEnabled = !Cursor.visible;
        mouseMovement = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        float xTurn = mouseMovement[0] * xSens * Time.deltaTime;
        float yTurn = mouseMovement[1] * ySens * Time.deltaTime;
        float yTurnPOV = mouseMovement[1] * ySensPOV * Time.deltaTime;

        if (DDOL)
        {
            xTurn *= DDOL.sensitivityMultiplier;
            yTurn *= DDOL.sensitivityMultiplier;
            yTurnPOV *= DDOL.sensitivityMultiplier;
        }

        if (turningEnabled)
        {
            transform.Rotate(Vector3.up, xTurn);

            camHeight -= yTurn;
            camHeight = Mathf.Clamp(camHeight, 0.1f, 2.2f);

            xRotationCam -= yTurnPOV;
            xRotationCam = Mathf.Clamp(xRotationCam, -90f, 75f);
        }
    }

    void handleCamera()
    {
        if (isFirstPerson)
        {
            PovCameraTransform.localEulerAngles = new Vector3(xRotationCam, 0f, 0f);
            return;
        }

        distMult += (distMultGoal - distMult) / 10f;

        camDist = -0.74f * Mathf.Pow(actualCamHeight - 1.12f, 2) + 3f;
        float currentDist = camDist;

        float targetHeight = camHeight;
        float targetDist = currentDist * distMult;

        Vector3 desiredCamPos = lookAt.position - transform.forward * targetDist + Vector3.up * targetHeight;
        Vector3 directionToCam = (desiredCamPos - lookAt.position).normalized;
        float maxDist = Vector3.Distance(lookAt.position, desiredCamPos);

        RaycastHit hit;
        bool hitSomething = Physics.Raycast(lookAt.position, directionToCam, out hit, maxDist);

        if (hitSomething)
        {
            float safeDist = hit.distance - 0.2f;
            safeDist = Mathf.Clamp(safeDist, currentDist * 0.2f, targetDist);
            actualCamDist = safeDist;
        }
        else
        {
            actualCamDist += (targetDist - actualCamDist) / 5f;
        }

        actualCamHeight += (camHeight - actualCamHeight) / 5f;

        if (cameraTransform)
        {
            cameraTransform.localPosition = new Vector3(0f, actualCamHeight, -actualCamDist);
            cameraTransform.LookAt(lookAt);
        }
    }

    void jumpOrDash()
    {
        if (isGrounded)
            playerVelocity.y = Mathf.Sqrt(jumpHeight * -2f * Physics.gravity.y);
    }
}
