using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy Stats", menuName = "AI/Enemy Stats")]
public class EnemyStats : ScriptableObject
{
    [Header("Basic Info")]
    public string enemyName = "Enemy";

    [Header("Health")]
    public float maxHealth = 100f;

    [Header("Movement")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 3f;

    [Header("Detection")]
    public float detectionRadius = 10f;
    public float loseTargetDistance = 15f;
    public LayerMask targetLayer;

    [Header("Combat")]
    public float attackRange = 2f;
    public float attackDamage = 10f;
    public float attackCooldown = 1.5f;

    [Header("Retreat")]
    public float retreatHealthThreshold = 0.3f;
    public float fleeDistance = 10f;

    [Header("Patrol")]
    public float wanderRadius = 8f;
}