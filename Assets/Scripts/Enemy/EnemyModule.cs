// EnemyModule.cs - Clase base para todos los módulos
using UnityEngine;

public abstract class EnemyModule : MonoBehaviour
{
    protected EnemyCore core;
    protected EnemySharedData sharedData;
    protected EnemyStats stats;

    [Header("Module Settings")]
    [SerializeField] protected bool isActive = true;
    [SerializeField] protected int priority = 0; // Para orden de ejecución

    public virtual void Initialize(EnemyCore enemyCore, EnemySharedData data)
    {
        core = enemyCore;
        sharedData = data;
        stats = enemyCore.Stats;
        OnInitialize();
    }

    protected virtual void OnInitialize() { }

    public virtual void OnUpdate() { }

    public virtual void OnFixedUpdate() { }

    public virtual void OnDamageTaken(float damage, Vector3 hitPoint, Vector3 hitDirection) { }

    public virtual void OnDeath() { }

    public virtual void DrawGizmos() { }

    protected void SetActive(bool active)
    {
        isActive = active;
    }
}