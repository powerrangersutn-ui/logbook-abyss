using System.Collections;
//using System.Runtime.ExceptionServices;
//using System.Numerics;
using UnityEngine;
//using UnityEngine.UIElements;

public class EnemyPatrol : MonoBehaviour
{
    [Header("Speed Between Waypoints")]
    [SerializeField] private float speed;
    
    //Here you set the Waypoints and Wait Time between Waypoints
    [Header("Waypoints/Rutines")]
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private float waitTime;
    [SerializeField] private float rotationSpeed = 5.3f;

    private int currentWaypoint;
    private bool isWaiting;

    //Is used in EnemyMovement to stop the patrol
    public bool playerDetected = false;

    void Start()
    {
        Vector3 direction = (waypoints[0].position - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(direction);
    }

    void Update()
    {
        
        //Just still patrol if the player hasnt been detected
        if (!playerDetected)
        {
            //If the enemy hasnt reached the current waypoint, move towards it
            if (Vector3.Distance(transform.position, waypoints[currentWaypoint].position) > 0.01f)
            {
                if (!isWaiting)
                {
                    transform.position = Vector3.MoveTowards(transform.position, waypoints[currentWaypoint].position, speed * Time.deltaTime);
                }
            }
            //This start the Coroutine between Waypoints if waiting
            else if (!isWaiting)
            {
                StartCoroutine(Wait());
            }
        }
    }

    IEnumerator Wait()
    {
        isWaiting = true;

        //"waitTime" defines how long the enemy waits between each waypoint
        yield return new WaitForSeconds(waitTime);

        int nextWaypoint;
        do  nextWaypoint = Random.Range(0, waypoints.Length);
        while (nextWaypoint == currentWaypoint);

        currentWaypoint = nextWaypoint;

        isWaiting = false;

        StartCoroutine(RotBetweenWaypoints(waypoints[currentWaypoint].position));
    }

    IEnumerator RotBetweenWaypoints(Vector3 target)
    {
        Vector3 direction = (target - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        while (Quaternion.Angle(transform.rotation, targetRotation) > 0.1f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            yield return null;
        }
    }

    
}
