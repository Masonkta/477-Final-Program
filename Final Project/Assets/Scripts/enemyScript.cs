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
    private Dictionary<string, Transform> pointCache;
    public Transform currentDestination;
    public float moveSpeed = 3f;
    float timeOfLastDestinationSet;
    int pointsVisited = 0;
    private float lastAIUpdateTime;

    private const float aiUpdateInterval = 0.1f; // Update AI every 0.1 seconds
    private const float destinationChangeCooldown = 3f;
    private const float reachThresholdSqr = 9f; // squared distance of 3 units

    // Start is called before the first frame update
    void Start()
    {   
        CacheDestinationPoints();
        controller = GetComponent<CharacterController>();
        currentDestination = pointCache["First Fork"];
        timeOfLastDestinationSet = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        MoveToCurrentPoint();
    }

    void CacheDestinationPoints()
    {
        pointCache = new Dictionary<string, Transform>();
        foreach (Transform child in transform.parent.Find("Points"))
        {
            pointCache[child.name] = child;
        }
    }

    void MoveToCurrentPoint()
    {
        if (!currentDestination) return;

        Vector3 directionToTarget = currentDestination.position - transform.position;
        directionToTarget.y = 0f;

        // Rotate toward the destination if necessary
        if (directionToTarget.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }

        // Simple movement (more efficient than CharacterController.Move)
        // transform.position += directionToTarget.normalized * moveSpeed * Time.deltaTime;
        controller.Move(directionToTarget.normalized * moveSpeed * Time.deltaTime);

        // Check if close enough to switch destinations
        if (Time.time - lastAIUpdateTime > aiUpdateInterval)
            if (directionToTarget.sqrMagnitude < reachThresholdSqr &&
                Time.time - timeOfLastDestinationSet >= destinationChangeCooldown)
            {
                SelectNewDestination();
                timeOfLastDestinationSet = Time.time;
            }
    }

    void SelectNewDestination()
    {
        string pointName = null;

        switch (direction)
        {
            case Direction.Left:
                if (pointsVisited == 5) pointsVisited = 1;
                if (pointsVisited == 0) pointName = "Left Point 1";
                else if (pointsVisited == 1) pointName = "Left Point 2";
                else if (pointsVisited == 2) pointName = "Back Of City";
                else if (pointsVisited == 3) pointName = "Left Point 4";
                else if (pointsVisited == 4) pointName = "Left Point 5";
                break;

            case Direction.Right:
                if (pointsVisited == 5) pointsVisited = 0;
                if (pointsVisited == 0) pointName = "Right Point 1";
                else if (pointsVisited == 1) pointName = "Right Point 2";
                else if (pointsVisited == 2) pointName = "Right Point 3";
                else if (pointsVisited == 3) pointName = "Right Point 4";
                else if (pointsVisited == 4) pointName = "Right Point 5";
                break;

            case Direction.Straight:
                if (pointsVisited == 3) pointsVisited = 0;
                if (pointsVisited == 0) pointName = "Straight Point 1";
                else if (pointsVisited == 1) pointName = "Straight Point 2";
                else if (pointsVisited == 2) pointName = "Straight Point 3";
                break;

            case Direction.Neighborhood:
                if (pointsVisited == 5) pointsVisited = 1;
                if (pointsVisited == 0) pointName = "Straight Point 1";
                else if (pointsVisited == 1) pointName = "To Neighborhood";
                else if (pointsVisited == 2) pointName = "Inside Neighborhood";
                else if (pointsVisited == 3) pointName = "Inside Neighborhood 2";
                else if (pointsVisited == 4) pointName = "Inside Neighborhood 3";
                break;
        }

        if (pointName != null && pointCache.TryGetValue(pointName, out Transform nextPoint))
        {
            currentDestination = nextPoint;
        }

        pointsVisited++;
    }
}
