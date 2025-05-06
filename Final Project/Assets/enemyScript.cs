using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyScript : MonoBehaviour
{   
    CharacterController controller;
    [Header("Points")]
    public Transform point1;

    [Header("Other")]
    public Transform currentDestination;
    public float moveSpeed;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        point1 = transform.parent.Find("Point 1");
        currentDestination = point1;
    }

    // Update is called once per frame
    void Update()
    {
        moveToCurrentPoint();
    }

    void moveToCurrentPoint(){
        Vector3 moveDirection = currentDestination.position - transform.position; moveDirection.y = 0f;

        controller.Move(moveDirection.normalized * moveSpeed * Time.deltaTime);
    }
}
