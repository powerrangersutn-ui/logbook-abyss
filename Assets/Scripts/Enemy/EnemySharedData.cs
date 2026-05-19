// EnemySharedData.cs - Datos compartidos entre módulos
using UnityEngine;

[System.Serializable]
public class EnemySharedData
{
    // Referencias
    public Transform transform;
    public EnemyCore core;

    // Estado de salud
    public float maxHealth;
    public float currentHealth;
    public bool isDead = false;

    // Estado de combate
    public Transform currentTarget;
    public float lastAttackTime;
    public Vector3 lastHitPoint;
    public Vector3 lastHitDirection;

    // Estado de movimiento
    public Vector3 desiredVelocity;
    public Vector3 desiredDirection;
    public float currentSpeed;

    // Estado del grupo/cardumen
    public int nearbyAlliesCount;
    public Vector3 groupCenter;

    // Flags de comportamiento
    public bool isRetreating;
    public bool isAggressive;
    public bool hasCalledForHelp;

    // Helpers
    public float HealthPercent => currentHealth / maxHealth;

    public bool HasTarget => currentTarget != null;

    public float DistanceToTarget => HasTarget ?
        Vector3.Distance(transform.position, currentTarget.position) : float.MaxValue;
}