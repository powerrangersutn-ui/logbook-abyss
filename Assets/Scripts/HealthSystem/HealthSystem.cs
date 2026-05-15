using UnityEngine;
using System;

//Es un Modelo. Solo sabe de números. Es reutilizable. (por ejemplo para Player y Enemy. Luego cada PlayerHealth y EnemyHealth se encarga de cómo usarlo)
public class HealthSystem : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;
    private bool isDead = false;

    public Action OnHealthChanged;
    public Action OnDeath;
    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;
    public bool IsAlive => currentHealth > 0 && !isDead;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        if (!IsAlive) return;

        currentHealth = Mathf.Max(0, currentHealth - damage);

        if (currentHealth <= 0)
        {
            isDead = true;
        }
    }

    public void Heal(int amount)
    {
        if (!IsAlive) return;
        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
    }

    public void ResetHealth()
    {
        isDead = false;
        currentHealth = maxHealth;
    }

    public void SetMaxHealth(int newMaxHealth)
    {
        if (newMaxHealth <= 0) return;
        maxHealth = newMaxHealth;

        if (currentHealth > maxHealth)
            currentHealth = maxHealth;

        OnHealthChanged?.Invoke();
    }
}