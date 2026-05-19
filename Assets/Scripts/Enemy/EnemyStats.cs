using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy Stats", menuName = "AI/Enemy Stats")]
public class EnemyStats : ScriptableObject
{
    [Header("Basic Stats")]
    public string enemyName;
    public float maxHealth = 100f;
    public float moveSpeed = 5f;
    public float rotationSpeed = 3f;

    [Header("Detection")]
    public float detectionRadius = 10f;
    public float attackRange = 5f;
    public float loseTargetDistance = 15f;
    public LayerMask targetLayer;

    [Header("Combat")]
    public float attackDamage = 10f;
    public float attackCooldown = 2f;
    public float retreatHealthThreshold = 0.3f; // 30% vida

    [Header("Behavior Specific")]
    public float wanderRadius = 8f;
    public float fleeDistance = 12f;
    public bool canCallAllies = false;
    public float allyCallRadius = 15f;
}