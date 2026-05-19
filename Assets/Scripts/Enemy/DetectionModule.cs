// DetectionModule.cs - Detecta objetivos
using UnityEngine;

public class DetectionModule : EnemyModule
{
    [Header("Detection Settings")]
    [SerializeField] private float detectionRadius = 10f;
    [SerializeField] private float loseTargetDistance = 15f;
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private float detectionInterval = 0.5f;

    [Header("Vision Settings")]
    [SerializeField] private bool useVisionCone = false;
    [SerializeField] private float visionAngle = 120f;

    private float detectionTimer;

    public override void OnUpdate()
    {
        if (!isActive) return;

        detectionTimer -= Time.deltaTime;

        if (detectionTimer <= 0)
        {
            UpdateTargetDetection();
            detectionTimer = detectionInterval;
        }

        // Verificar si perdimos el objetivo
        if (sharedData.HasTarget)
        {
            float distance = sharedData.DistanceToTarget;
            if (distance > loseTargetDistance)
            {
                sharedData.currentTarget = null;
            }
        }
    }

    private void UpdateTargetDetection()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius, targetLayer);

        Transform closestTarget = null;
        float closestDistance = float.MaxValue;

        foreach (Collider hit in hits)
        {
            if (hit.transform == transform) continue;

            float distance = Vector3.Distance(transform.position, hit.transform.position);

            // Si usamos cono de visión, verificar ángulo
            if (useVisionCone)
            {
                Vector3 directionToTarget = (hit.transform.position - transform.position).normalized;
                float angle = Vector3.Angle(transform.forward, directionToTarget);

                if (angle > visionAngle / 2f)
                {
                    continue; // Fuera del cono de visión
                }
            }

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestTarget = hit.transform;
            }
        }

        if (closestTarget != null)
        {
            sharedData.currentTarget = closestTarget;
        }
    }

    public override void DrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, loseTargetDistance);

        if (useVisionCone && Application.isPlaying)
        {
            Gizmos.color = new Color(1, 1, 0, 0.3f);
            Vector3 leftBoundary = Quaternion.Euler(0, -visionAngle / 2f, 0) * transform.forward * detectionRadius;
            Vector3 rightBoundary = Quaternion.Euler(0, visionAngle / 2f, 0) * transform.forward * detectionRadius;

            Gizmos.DrawLine(transform.position, transform.position + leftBoundary);
            Gizmos.DrawLine(transform.position, transform.position + rightBoundary);
        }
    }
}