using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemyPatrol : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float waitTime;
    [SerializeField] private Transform[] waypoints;
    private int currentWaypoint;
    private bool isWaiting;
    void Start()
    {
        
    }

    void Update()
    {
        if (Vector3.Distance(transform.position, waypoints[currentWaypoint].position) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, waypoints[currentWaypoint].position, speed * Time.deltaTime);
        }
        else if (!isWaiting)
        {
            StartCoroutine(Wait());
        }
    }

    IEnumerator Wait()
    {
        isWaiting = true;
        yield return new WaitForSeconds(waitTime);
        currentWaypoint++;
        if(currentWaypoint == waypoints.Length)
        {
            currentWaypoint = 0;
        }
        isWaiting = false;
    }
}
