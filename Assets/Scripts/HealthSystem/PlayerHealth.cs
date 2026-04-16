using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : MonoBehaviour
{
    [Header("Sistema de Vidas")]
    [SerializeField] private int maxLives = 3;

    [Header("Configuración de Daño")]
    [SerializeField] private float damageCooldown = 0.6f;     // Tiempo de invulnerabilidad después de recibir daño

    [Header("Eventos")]
    public UnityEvent OnTakeDamage;
    public UnityEvent OnLifeLost;
    public UnityEvent OnPlayerDeath;

    private HealthSystem healthSystem;
    private Rigidbody playerRb;
    private float lastDamageTime;
    private int currentLives;

    // Propiedades públicas
    public int CurrentLives => currentLives;
    public bool IsInvulnerable => Time.time - lastDamageTime < damageCooldown;

    private void Awake()
    {
        InitializeComponents();
        currentLives = maxLives;
        lastDamageTime = -damageCooldown;        // Para poder recibir daño desde el primer segundo
    }

    private void InitializeComponents()
    {
        healthSystem = GetComponent<HealthSystem>();
        if (healthSystem == null)
            healthSystem = gameObject.AddComponent<HealthSystem>();

        playerRb = GetComponent<Rigidbody>();
    }

    #region Daño General (Trigger / Ataques)
    public void TakeDamage(int damage)
    {
        if (IsInvulnerable || !healthSystem.IsAlive)
        {
            // Debug.Log("Daño ignorado por invulnerabilidad");   // Descomenta si querés debug
            return;
        }

        lastDamageTime = Time.time;

        healthSystem.TakeDamage(damage);
        OnTakeDamage?.Invoke();

        if (healthSystem.CurrentHealth <= 0)
        {
            LoseLife();
        }
        else
        {
            healthSystem.ResetHealth();     // Resetear la barra de vida por cada "vida" perdida
        }
    }
    #endregion

    private void LoseLife()
    {
        currentLives--;
        OnLifeLost?.Invoke();

        if (currentLives <= 0)
        {
            Die();
        }
        else
        {
            healthSystem.ResetHealth();
        }
    }

    private void Die()
    {
        OnPlayerDeath?.Invoke();
        Debug.Log("Jugador murió");

        if (GameManager.Instance != null)
        {
            GameManager.Instance.PlayerDied();
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    #region Métodos Públicos
    public void Heal(int amount)
    {
        healthSystem?.Heal(amount);
    }

    public void AddLife()
    {
        if (currentLives < maxLives)
            currentLives++;
    }

    public float GetHealthPercentage() => healthSystem.HealthPercentage;
    #endregion
}