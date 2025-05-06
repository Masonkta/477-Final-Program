using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum Direction
{
    Straight,
    Left,
    Right,
    Neighborhood
}

public class enemyScript : MonoBehaviour
{   
    CharacterController controller;
    [Header("Points")]
    public Direction direction;

    [Header("Other")]
    public Transform currentDestination;
    public float moveSpeed = 3f;
    float timeOfLastDestinationSet;
    int pointsVisited = 0;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        currentDestination = transform.parent.Find("First Fork");
        timeOfLastDestinationSet = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        moveToCurrentPoint();
    }

    void moveToCurrentPoint(){
        Vector3 moveDirection = currentDestination.position - transform.position; moveDirection.y = 0f;

        // Only rotate if we're actually moving
        if (moveDirection.sqrMagnitude > 0.001f) {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }

        controller.Move(moveDirection.normalized * moveSpeed * Time.deltaTime);
                                                                    //  Dist from point
        if (Vector3.Distance(transform.position, currentDestination.position) < 2f && Time.time - timeOfLastDestinationSet > 2f){ // Second timeout before destination updates can be made
            selectNewDestination();
            timeOfLastDestinationSet = Time.time;
        }
    }

    void selectNewDestination(){
        if (direction == Direction.Left){
            if (pointsVisited == 0) currentDestination = transform.parent.Find("Left Point 1");
            if (pointsVisited == 1) currentDestination = transform.parent.Find("Left Point 2");
            if (pointsVisited == 2) currentDestination = transform.parent.Find("Back Of City");
            pointsVisited += 1;
        }

        if (direction == Direction.Right){
            if (pointsVisited == 0) currentDestination = transform.parent.Find("Right Point 1");
            if (pointsVisited == 1) currentDestination = transform.parent.Find("Right Point 2");
            pointsVisited += 1;
        }
        
        if (direction == Direction.Straight){
            if (pointsVisited == 0) currentDestination = transform.parent.Find("Straight Point 1");
            if (pointsVisited == 1) currentDestination = transform.parent.Find("Straight Point 2");
            
            pointsVisited += 1;
        }
        
        if (direction == Direction.Neighborhood){
            if (pointsVisited == 0) currentDestination = transform.parent.Find("Straight Point 1");
            if (pointsVisited == 1) currentDestination = transform.parent.Find("To Neighborhood");
            if (pointsVisited == 2) currentDestination = transform.parent.Find("Inside Neighborhood");
            
            pointsVisited += 1;
        }
    }
}
