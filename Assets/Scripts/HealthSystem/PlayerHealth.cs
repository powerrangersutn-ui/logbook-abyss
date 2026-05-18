using UnityEngine;

//Es un Controlador. Decide cuándo se hace daño y utiliza HealthSystem para ejecutarlo.
//Avisa a quienes estén suscriptos por medio de UIGameEvents.

public class PlayerHealth : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private int maxLives = 3;
    [SerializeField] private float damageCooldown = 0.5f;

    private HealthSystem healthSystem;
    private Rigidbody playerRb;
    private float lastDamageTime;
    private int currentLives;
    public bool IsDead { get; private set; } = false;

    private bool IsInvulnerable => Time.time - lastDamageTime < damageCooldown;

    private void Awake()
    {
        healthSystem = GetComponent<HealthSystem>() ?? gameObject.AddComponent<HealthSystem>();
        playerRb = GetComponent<Rigidbody>();
        currentLives = maxLives;
    }

    private void Start()
    {
        // Notificamos la vida inicial al comenzar
        UIGameEvents.OnPlayerHealthChanged(healthSystem.CurrentHealth, healthSystem.MaxHealth);
    }

    public void TakeDamage(int damage)
    {
        if (IsDead || IsInvulnerable) return;

        lastDamageTime = Time.time;
        healthSystem.TakeDamage(damage);

        UIGameEvents.OnPlayerHealthChanged(healthSystem.CurrentHealth, healthSystem.MaxHealth);

        if (healthSystem.CurrentHealth <= 0)
        {
            // LoseLife(); 

            // Por ahora, muere directo al llegar a 0 vida
            Die();
        }
    }

    private void Die()
    {
        if (IsDead) return;
        IsDead = true;

        if (playerRb != null)
        {
            playerRb.linearVelocity = Vector3.zero;
            playerRb.isKinematic = true;
        }

        UIGameEvents.onPlayerDeath();
    }

    /* 
    private void LoseLife() 
    {
        currentLives--;
        if (currentLives <= 0) Die();
        else {
            healthSystem.ResetHealth();
            UIGameEvents.SendHealthChanged(healthSystem.CurrentHealth, healthSystem.MaxHealth);
        }
    }
    */

    public void Heal(int amount)
    {
        if (IsDead) return;
        healthSystem.Heal(amount);
        UIGameEvents.OnPlayerHealthChanged(healthSystem.CurrentHealth, healthSystem.MaxHealth);
    }
}