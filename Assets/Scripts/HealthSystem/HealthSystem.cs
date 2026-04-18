using UnityEngine;
using UnityEngine.Events;

public class HealthSystem : MonoBehaviour
{
    [Header("Configuración de Vida")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private bool destroyOnDeath = false;

    [Header("Eventos")]
    public UnityEvent OnDamageTaken;
    public UnityEvent OnHealthChanged;
    public UnityEvent OnDeath;

    private int currentHealth;
    private bool isDead = false;                    // ← Nueva variable de control

    // Propiedades públicas
    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;
    public float HealthPercentage => (float)currentHealth / maxHealth;
    public bool IsAlive => currentHealth > 0 && !isDead;

    private void Awake()
    {
        currentHealth = maxHealth;
        isDead = false;
    }

    public void TakeDamage(int damage)
    {
        if (!IsAlive) return;

        currentHealth = Mathf.Max(0, currentHealth - damage);

        OnDamageTaken?.Invoke();
        OnHealthChanged?.Invoke();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        if (!IsAlive) return;

        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        OnHealthChanged?.Invoke();
    }

    public void ResetHealth()
    {
        if (isDead) return;           // ← No permitir resetear si ya murió

        currentHealth = maxHealth;
        OnHealthChanged?.Invoke();
    }

    public void SetMaxHealth(int newMaxHealth)
    {
        if (newMaxHealth <= 0) return;
        maxHealth = newMaxHealth;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
        OnHealthChanged?.Invoke();
    }

    protected virtual void Die()
    {
        if (isDead) return;

        isDead = true;
        OnDeath?.Invoke();
        Debug.Log($"[HealthSystem] {gameObject.name} ha muerto");
    }

    // Método útil para saber si está realmente muerto
    public bool IsDead() => isDead;
}