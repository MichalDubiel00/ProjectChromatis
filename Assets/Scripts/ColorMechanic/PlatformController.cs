using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformController : MonoBehaviour
{
    [SerializeField] Vector3[] localWaypoints;
    Vector3[] globalWaypoints;
    int globalWaypointsIndex = 0;
    int pointIndex = 1;
    int direction = 1;
    Vector3 moveDirection;
    Vector3 targetPos;

    [SerializeField] float platformSpeed = 1f;
    public bool moveOn = false;

    Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Initialize global waypoints
        globalWaypoints = new Vector3[localWaypoints.Length];
        for (int i = 0; i < localWaypoints.Length; i++)
        {
            globalWaypoints[i] = localWaypoints[i] + transform.position;
        }
        targetPos = globalWaypoints[1];
        globalWaypointsIndex = globalWaypoints.Length - 1;
        CalculateDirection();
    }

    void Update()
    {
        if (moveOn)
        {
            // Check if close enough to the target waypoint
            if ((transform.position - targetPos).sqrMagnitude < 0.01f)
            {
                NextPoint();
            }

            // Apply velocity
            rb.velocity = moveDirection * platformSpeed;
        }
        else
        {
            rb.velocity = Vector2.zero; // Stop the platform when not moving
        }
    }

    void NextPoint()
    {
        // Reverse direction at the ends of the waypoint array
        if (pointIndex == globalWaypointsIndex)
            direction = -1;
        if (pointIndex == 0)
            direction = 1;

        // Update the next target position
        pointIndex += direction;
        targetPos = globalWaypoints[pointIndex];
        CalculateDirection();
    }

    void CalculateDirection()
    {
        // Calculate normalized direction towards the target position
        moveDirection = (targetPos - transform.position).normalized;
    }



    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Hello");

        PlayerMovement player = collision.GetComponent<PlayerMovement>();
        if (player != null)
        {
            Debug.Log("World");
            player.RB.gravityScale = 50;
            player.platformRB = rb;
            player.isOnPlatform = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        Debug.Log("Hello");

        PlayerMovement player = collision.gameObject.GetComponent<PlayerMovement>();
        if (player != null)
        {
            Debug.Log("World");

            player.isOnPlatform = false;
        }
 
    }

    private void OnDrawGizmos()
    {
        if(localWaypoints != null)
        {
            Gizmos.color = Color.red;
            float size = .3f;

            for (int i = 0; i < localWaypoints.Length; i++) 
            {
                Vector3 globalWaypointPos = !Application.isPlaying?(localWaypoints[i] + transform.position):globalWaypoints[i];
                Gizmos.DrawLine(globalWaypointPos - Vector3.up * size,globalWaypointPos + Vector3.up *size);
                Gizmos.DrawLine(globalWaypointPos - Vector3.left * size,globalWaypointPos + Vector3.left *size);
            }
        }
    }

}
