// EnemyCore.cs - N�cleo del enemigo (reemplaza BaseEnemyAI)
using UnityEngine;
using System.Collections.Generic;

public class EnemyCore : MonoBehaviour
{
    [Header("Core Stats")]
    [SerializeField] private EnemyStats stats;

    [Header("Modules")]
    [SerializeField] private List<EnemyModule> modules = new List<EnemyModule>();

    // Estado compartido entre m�dulos
    private EnemySharedData sharedData = new EnemySharedData();

    private Rigidbody rb;
    private Collider mainCollider;

    public EnemyStats Stats => stats;
    public EnemySharedData SharedData => sharedData;
    public Rigidbody Rigidbody => rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        mainCollider = GetComponent<Collider>();

        sharedData.maxHealth = stats.maxHealth;
        sharedData.currentHealth = stats.maxHealth;
        sharedData.transform = transform;
        sharedData.core = this;

        // Configurar f�sica para agua
        rb.linearDamping = 1f;
        rb.angularDamping = 2f;

        // Inicializar m�dulos
        InitializeModules();
    }

    private void InitializeModules()
    {
        // Obtener todos los m�dulos del GameObject
        modules.Clear();
        modules.AddRange(GetComponents<EnemyModule>());

        foreach (var module in modules)
        {
            module.Initialize(this, sharedData);
        }
    }

    private void Update()
    {
        // Actualizar todos los m�dulos activos
        foreach (var module in modules)
        {
            if (module.enabled)
            {
                module.OnUpdate();
            }
        }
    }

    private void FixedUpdate()
    {
        // F�sica de todos los m�dulos
        foreach (var module in modules)
        {
            if (module.enabled)
            {
                module.OnFixedUpdate();
            }
        }
    }

    public void TakeDamage(float damage, Vector3 hitPoint, Vector3 hitDirection)
    {
        sharedData.currentHealth -= damage;
        sharedData.lastHitPoint = hitPoint;
        sharedData.lastHitDirection = hitDirection;

        // Notificar a todos los m�dulos
        foreach (var module in modules)
        {
            if (module.enabled)
            {
                module.OnDamageTaken(damage, hitPoint, hitDirection);
            }
        }

        if (sharedData.currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        sharedData.isDead = true;

        // Notificar m�dulos de muerte
        foreach (var module in modules)
        {
            if (module.enabled)
            {
                module.OnDeath();
            }
        }

        Destroy(gameObject, 2f);
    }

    public T GetModule<T>() where T : EnemyModule
    {
        return GetComponent<T>();
    }

    private void OnDrawGizmos()
    {
        // Cada m�dulo puede dibujar sus propios gizmos
        if (modules != null)
        {
            foreach (var module in modules)
            {
                if (module != null && module.enabled)
                {
                    module.DrawGizmos();
                }
            }
        }
    }
}