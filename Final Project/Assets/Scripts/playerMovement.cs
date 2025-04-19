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
    public Animator playerAnimator;
    public bool isFirstPerson;

    [Header("Moving")]
    public bool isGrounded;
    public float walkSpeed = 8f;
    public float runSpeed = 18f;
    float moveSpeed;
    public float actualMoveSpeed;
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





    // Start is called before the first frame update
    void Start()
    {
        assignPlayerComponents();

        isFirstPerson = PovCameraTransform;
    }

    void assignPlayerComponents(){
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

    // Update is called once per frame
    void Update()
    {
        if (Time.timeSinceLevelLoad > 0.1f)
        {
            handleInput();
            moving();
            turning();
        }

        handleCamera();
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////





    void handleInput()
    {
        sprinting = (Input.GetKey(KeyCode.LeftShift));

        if (Input.GetKeyDown(KeyCode.Space)) jumpOrDash();


        distMultGoal -= Input.mouseScrollDelta.y / 25f;
        distMultGoal = Mathf.Clamp(distMultGoal, 0.2f, 1.5f);
    }


    void moving()
    {
        moveSpeed = sprinting ? runSpeed : walkSpeed;
        movement = forwardTransform.TransformDirection(new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical")));
        if (movement.magnitude == 0) moveSpeed = 0f;


        actualMoveSpeed += (moveSpeed - actualMoveSpeed) / 10f;
        playerAnimator.SetFloat("Speed", actualMoveSpeed / runSpeed);
        playerAnimator.speed = 0.8f + actualMoveSpeed / runSpeed * 0.5f;

        controller.Move(movement * actualMoveSpeed * Time.deltaTime);



        if (!isGrounded) playerVelocity.y += Time.deltaTime * Physics.gravity.y;
        controller.Move(playerVelocity * Time.deltaTime);


        // jumpHeight = -1 / ((actualMoveSpeed / 10f) + 0.3f) + 4.833f;

        if (LayerMask.NameToLayer("Ground") == -1) // No Ground Layer Established
            isGrounded = (controller.isGrounded || Physics.Raycast(transform.position, Vector3.down, groundCheckDistance));
        else {                                     // Better, Ground Layer only if it exists
            int groundLayer = LayerMask.NameToLayer("Ground");
            isGrounded = (controller.isGrounded || Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, 1 << groundLayer)); //controller.isGrounded || 
        }

        if (isGrounded)
        {
            playerVelocity = Vector3.down * Mathf.Max(3f, -playerVelocity.y);
            //print("GROUNDED");
        }

    }

    void turning()
    {
        turningEnabled = !Cursor.visible;
        mouseMovement = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        ///////////////////////////////////////////////////////////

        float xTurn = mouseMovement[0] * xSens * Time.deltaTime;
        float yTurn = mouseMovement[1] * ySens * Time.deltaTime;
        float yTurnPOV = mouseMovement[1] * ySensPOV * Time.deltaTime;

        if (turningEnabled)
        {
            // Left + Right
            transform.Rotate(Vector3.up, xTurn);

            // Up + Down
            camHeight -= yTurn;
            camHeight = Mathf.Clamp(camHeight, 0.1f, 2.2f);

            xRotationCam -= yTurnPOV;
            xRotationCam = Mathf.Clamp(xRotationCam, -90f, 75f);
        }

        ///////////////////////////////////////////////////////////

    }

    void handleCamera()
    {
        // DECIDE WHICH CAM IS ACTIVE
        // cameraTransform.gameObject.SetActive(distMult > 0.45f); if (distMult < 0.45f) heightAbove = 2f;
        // PovCameraTransform.gameObject.SetActive(!cameraTransform.gameObject.activeInHierarchy); if (distMult > 0.45f) xRotationCam = 0f;
        //

        if (isFirstPerson){
            
            // POV Camera
            PovCameraTransform.localEulerAngles = new Vector3(xRotationCam, 0f, 0f);
            return;
        }

        // Third Person Stuff
        distMult += (distMultGoal - distMult) / 10f;

        // DO NOT BE INSIDE A WALL
        if (cameraTransform){
            float distToCam = Vector3.Distance(transform.position, cameraTransform.position);
            Vector3 toCamera = Vector3.Normalize(cameraTransform.position - transform.position); RaycastHit hit;
            if (isGrounded && Physics.Raycast(transform.position, toCamera, out hit, distToCam * 1.25f)) // Check a bit farther
            {
                float DistToObject = Vector3.Distance(hit.point, transform.position) * 1.5f; // Pull camera in a bit from the point
                camDist += (DistToObject - camDist) / 3f;
            }
        }
        //

        /// LEAVE ///
        // camHeight = -1.46f * heightAbove + 4.45f;
        // camDist = -Mathf.Pow((heightAbove + 1.5f) * 0.3f, 2) + 10f;
        camDist = -0.74f * Mathf.Pow(actualCamHeight - 1.12f, 2) + 3f; 
        /////////////

        actualCamHeight += (camHeight - actualCamHeight) / 5f;
        actualCamDist += (camDist - actualCamDist) / 5f;

        if (cameraTransform) {
            cameraTransform.localPosition = new Vector3(0f, actualCamHeight, -actualCamDist * distMult);
            // GetComponent<MeshRenderer>().enabled = distMult > 0.4f;
            
            cameraTransform.LookAt(lookAt);
        }
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////// 

    void jumpOrDash()
    {
        if (isGrounded)
            playerVelocity.y = Mathf.Sqrt(jumpHeight * -2f * Physics.gravity.y);



    }



    
}
