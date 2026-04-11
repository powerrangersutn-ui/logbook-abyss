using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemyPatrol : MonoBehaviour
{
    [Header("Speed Between Waypoints")]
    [SerializeField] private float speed;
    
    //Here you set the Waypoints and Wait Time between Waypoints
    [Header("Waypoints/Rutines")]
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private float waitTime;
    [SerializeField] private float rotationSpeed = 5f;

    private int currentWaypoint;
    private bool isWaiting;

    //Is used in EnemyMovement to stop the patrol
    public bool playerDetected = false;

    void Start()
    {
        
    }

    void Update()
    {
        //Just still patrol if the player hasnt been detected
        if (!playerDetected)
        {
            //If the enemy hasnt reached the current waypoint, move towards it
            if (Vector3.Distance(transform.position, waypoints[currentWaypoint].position) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, waypoints[currentWaypoint].position, speed * Time.deltaTime);
                Vector3 direction = (waypoints[currentWaypoint].position - transform.position).normalized;
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
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
        currentWaypoint++;
        //Restart to the first Waypoint (0)
        if(currentWaypoint == waypoints.Length)
        {
            currentWaypoint = 0;
        }
        isWaiting = false;
    }
}
