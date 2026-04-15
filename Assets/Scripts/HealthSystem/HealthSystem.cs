using UnityEngine;
using UnityEngine.Events;

public class HealthSystem : MonoBehaviour
{
    [Header("Configuración de Vida")]
    [SerializeField] private int maxHealth = 3;
    [SerializeField] private bool destroyOnDeath = true;
    [SerializeField] private float destroyDelay = 0.15f;  //delay antes de destruir

    [Header("Eventos")]
    public UnityEvent OnDamageTaken;
    public UnityEvent OnDeath;

    private int currentHealth;

    // Propiedades públicas
    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;
    public float HealthPercentage => (float)currentHealth / maxHealth;
    public bool IsAlive => currentHealth > 0;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void SetMaxHealth(int newMaxHealth)
    {
        if (newMaxHealth <= 0) return;
        maxHealth = newMaxHealth;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
    }

    public void TakeDamage(int damage)
    {
        if (!IsAlive) return;

        currentHealth = Mathf.Max(0, currentHealth - damage);
        OnDamageTaken?.Invoke();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
    }

    protected virtual void Die()
    {
        OnDeath?.Invoke();  //Esto llama a ReleaseStuckHarpoons PRIMERO

        if (destroyOnDeath)
        {
            //Esperamos un poco antes de destruir
            Destroy(gameObject, destroyDelay);
        }
    }
}