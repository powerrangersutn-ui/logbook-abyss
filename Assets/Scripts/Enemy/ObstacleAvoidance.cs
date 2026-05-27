using UnityEngine;

public class ObstacleAvoidance : MonoBehaviour
{
    [Header("Detection")]
    [SerializeField] private float detectionDistance = 3f;

    [SerializeField] private float sphereRadius = 0.5f;

    [SerializeField] private LayerMask obstacleMask;

    [Header("Avoidance")]
    [SerializeField] private float avoidanceStrength = 5f;

    public Vector3 GetAvoidanceDirection()
    {
        Vector3 avoidance = Vector3.zero;

        CheckDirection(transform.forward, ref avoidance);
        CheckDirection((transform.forward + transform.right).normalized, ref avoidance);
        CheckDirection((transform.forward - transform.right).normalized, ref avoidance);

        CheckDirection((transform.forward + transform.up).normalized, ref avoidance);
        CheckDirection((transform.forward - transform.up).normalized, ref avoidance);

        return avoidance.normalized;
    }

    private void CheckDirection(Vector3 direction, ref Vector3 avoidance)
    {
        if (Physics.SphereCast(transform.position,
                sphereRadius,
                direction,
                out RaycastHit hit,
                detectionDistance,
                obstacleMask))
        {
            Vector3 away = (transform.position - hit.point).normalized;

            avoidance += away * avoidanceStrength;
        }
    }
}