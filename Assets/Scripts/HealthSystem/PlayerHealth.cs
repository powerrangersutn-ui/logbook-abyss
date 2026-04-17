using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : MonoBehaviour
{
    [Header("Sistema de Vidas")]
    [SerializeField] private int maxLives = 3;
    [SerializeField] private GameObject gameOverPanel;

    [Header("Configuración de Daño")]
    [SerializeField] private float damageCooldown = 0.6f;     // Tiempo de invulnerabilidad

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
        lastDamageTime = -1f;           // Para poder recibir daño inmediatamente
    }

    private void InitializeComponents()
    {
        healthSystem = GetComponent<HealthSystem>() ?? gameObject.AddComponent<HealthSystem>();
        playerRb = GetComponent<Rigidbody>();
    }

    #region Daño desde Ataques (Trigger del arma enemiga)
    public void TakeDamage(int damage)
    {
        if (IsInvulnerable || !healthSystem.IsAlive)
            return;

        lastDamageTime = Time.time;
        healthSystem.TakeDamage(damage);
        OnTakeDamage?.Invoke();

        if (healthSystem.CurrentHealth <= 0)
        {
            LoseLife();
        }
        else
        {
            healthSystem.ResetHealth();
        }
    }
    #endregion

    private void LoseLife()
    {
        currentLives--;
        OnLifeLost?.Invoke();

        if (currentLives <= 0)
        {
            gameOverPanel.SetActive(true);
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

        // Desactivamos física para que no se mueva más
        if (playerRb != null)
            playerRb.isKinematic = true;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.PlayerDied();
        }
        else
        {
            //gameObject.SetActive(false);
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