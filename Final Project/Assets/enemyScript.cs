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
    float timeOfLastAICheck;
    private const float destinationChangeCooldown = 2f;
    private const float reachThresholdSqr = 4f; // squared distance of 2 units

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
        transform.position += directionToTarget.normalized * moveSpeed * Time.deltaTime;

        // Check if close enough to switch destinations
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
                if (pointsVisited == 0) pointName = "Left Point 1";
                else if (pointsVisited == 1) pointName = "Left Point 2";
                else if (pointsVisited == 2) pointName = "Back Of City";
                break;

            case Direction.Right:
                if (pointsVisited == 0) pointName = "Right Point 1";
                else if (pointsVisited == 1) pointName = "Right Point 2";
                break;

            case Direction.Straight:
                if (pointsVisited == 0) pointName = "Straight Point 1";
                else if (pointsVisited == 1) pointName = "Straight Point 2";
                break;

            case Direction.Neighborhood:
                if (pointsVisited == 0) pointName = "Straight Point 1";
                else if (pointsVisited == 1) pointName = "To Neighborhood";
                else if (pointsVisited == 2) pointName = "Inside Neighborhood";
                break;
        }

        if (pointName != null && pointCache.TryGetValue(pointName, out Transform nextPoint))
        {
            currentDestination = nextPoint;
        }

        pointsVisited++;
    }
}
