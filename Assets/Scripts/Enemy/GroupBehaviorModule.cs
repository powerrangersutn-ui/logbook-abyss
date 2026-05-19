// GroupBehaviorModule.cs - Comportamiento de grupo/cardumen
using UnityEngine;
using System.Collections.Generic;

public class GroupBehaviorModule : EnemyModule
{
    [Header("Group Settings")]
    [SerializeField] private float groupRadius = 8f;
    [SerializeField] private float groupCheckInterval = 1f;
    [SerializeField] private LayerMask allyLayer;

    [Header("Flocking Behavior")]
    [SerializeField] private bool useFlocking = true;
    [SerializeField] private float separationWeight = 1.5f;
    [SerializeField] private float alignmentWeight = 1f;
    [SerializeField] private float cohesionWeight = 1f;
    [SerializeField] private float separationDistance = 2f;

    [Header("Group Combat")]
    [SerializeField] private bool groupAggression = true;
    [SerializeField] private float aggressionBonus = 0.3f; // 30% más agresivo en grupo
    [SerializeField] private bool shareTargets = true;

    private List<EnemyCore> nearbyAllies = new List<EnemyCore>();
    private float groupCheckTimer = 0f;
    private MovementModule movementModule;

    protected override void OnInitialize()
    {
        movementModule = core.GetModule<MovementModule>();
    }

    public override void OnUpdate()
    {
        if (!isActive) return;

        // Actualizar lista de aliados periódicamente
        groupCheckTimer -= Time.deltaTime;
        if (groupCheckTimer <= 0)
        {
            UpdateNearbyAllies();
            groupCheckTimer = groupCheckInterval;
        }

        // Actualizar datos compartidos
        sharedData.nearbyAlliesCount = nearbyAllies.Count;

        if (nearbyAllies.Count > 0)
        {
            CalculateGroupCenter();

            // Aplicar comportamiento de flocking si está activo
            if (useFlocking && movementModule != null && !sharedData.HasTarget)
            {
                ApplyFlockingBehavior();
            }

            // Compartir objetivos
            if (shareTargets && sharedData.HasTarget)
            {
                ShareTargetWithAllies();
            }

            // Modificar agresividad
            if (groupAggression)
            {
                sharedData.isAggressive = true;
            }
        }
        else
        {
            sharedData.isAggressive = false;
        }
    }

    private void UpdateNearbyAllies()
    {
        nearbyAllies.Clear();
        Collider[] colliders = Physics.OverlapSphere(transform.position, groupRadius, allyLayer);

        foreach (Collider col in colliders)
        {
            EnemyCore ally = col.GetComponent<EnemyCore>();
            if (ally != null && ally != core)
            {
                nearbyAllies.Add(ally);
            }
        }
    }

    private void CalculateGroupCenter()
    {
        Vector3 center = transform.position;
        foreach (var ally in nearbyAllies)
        {
            if (ally != null)
            {
                center += ally.transform.position;
            }
        }
        center /= (nearbyAllies.Count + 1);
        sharedData.groupCenter = center;
    }

    private void ApplyFlockingBehavior()
    {
        Vector3 separation = CalculateSeparation();
        Vector3 alignment = CalculateAlignment();
        Vector3 cohesion = CalculateCohesion();

        Vector3 flockingForce =
            separation * separationWeight +
            alignment * alignmentWeight +
            cohesion * cohesionWeight;

        if (flockingForce != Vector3.zero)
        {
            sharedData.desiredVelocity += flockingForce.normalized * 0.3f;
        }
    }

    private Vector3 CalculateSeparation()
    {
        Vector3 separationForce = Vector3.zero;
        int count = 0;

        foreach (var ally in nearbyAllies)
        {
            if (ally == null) continue;

            float distance = Vector3.Distance(transform.position, ally.transform.position);
            if (distance < separationDistance && distance > 0)
            {
                Vector3 diff = transform.position - ally.transform.position;
                diff = diff.normalized / distance; // Más fuerte cuando más cerca
                separationForce += diff;
                count++;
            }
        }

        if (count > 0)
        {
            separationForce /= count;
        }

        return separationForce;
    }

    private Vector3 CalculateAlignment()
    {
        Vector3 averageVelocity = Vector3.zero;
        int count = 0;

        foreach (var ally in nearbyAllies)
        {
            if (ally == null) continue;

            averageVelocity += ally.Rigidbody.linearVelocity;
            count++;
        }

        if (count > 0)
        {
            averageVelocity /= count;
            return averageVelocity.normalized;
        }

        return Vector3.zero;
    }

    private Vector3 CalculateCohesion()
    {
        if (nearbyAllies.Count == 0) return Vector3.zero;

        Vector3 toCenter = sharedData.groupCenter - transform.position;
        return toCenter.normalized;
    }

    private void ShareTargetWithAllies()
    {
        foreach (var ally in nearbyAllies)
        {
            if (ally == null) continue;

            // Solo compartir si el aliado no tiene objetivo
            if (!ally.SharedData.HasTarget)
            {
                ally.SharedData.currentTarget = sharedData.currentTarget;
            }
        }
    }

    public void CallForHelp()
    {
        if (!sharedData.HasTarget) return;

        foreach (var ally in nearbyAllies)
        {
            if (ally == null) continue;

            ally.SharedData.currentTarget = sharedData.currentTarget;
        }

        sharedData.hasCalledForHelp = true;
    }

    public int GetGroupSize()
    {
        return nearbyAllies.Count + 1; // +1 para incluirse a sí mismo
    }

    public List<EnemyCore> GetNearbyAllies()
    {
        return nearbyAllies;
    }

    public override void DrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, groupRadius);

        if (Application.isPlaying && nearbyAllies.Count > 0)
        {
            // Dibujar conexiones con aliados
            Gizmos.color = new Color(0, 1, 1, 0.3f);
            foreach (var ally in nearbyAllies)
            {
                if (ally != null)
                {
                    Gizmos.DrawLine(transform.position, ally.transform.position);
                }
            }

            // Dibujar centro del grupo
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(sharedData.groupCenter, 1f);
        }
    }
}