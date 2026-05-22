using UnityEngine;

public class EnemySensors : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Vision")]
    [SerializeField] private float detectionRange = 15f;
    [SerializeField] private float visionAngle = 120f;

    [Header("Obstacle")]
    [SerializeField] private LayerMask obstacleMask;

    public bool CanSeeTarget { get; private set; }
    public Transform Target => target;

    private void Update()
    {
        EvaluateVision();
    }

    private void EvaluateVision()
    {
        CanSeeTarget = false;

        if (target == null)
            return;

        Vector3 direction = target.position - transform.position;

        if (direction.magnitude > detectionRange)
            return;

        float angle = Vector3.Angle(transform.forward, direction);

        if (angle > visionAngle * 0.5f)
            return;

        if (Physics.Raycast(transform.position, direction.normalized, out RaycastHit hit, detectionRange, obstacleMask))
        {
            if (hit.transform != target)
                return;
        }

        CanSeeTarget = true;
    }
}