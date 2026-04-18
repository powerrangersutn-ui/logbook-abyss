using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : MonoBehaviour
{
    [Header("Sistema de Vidas")]
    [SerializeField] private int maxLives = 3;

    [Header("Configuración de Daño")]
    [SerializeField] private float damageCooldown = 0.5f;

    [Header("Eventos")]
    public UnityEvent OnTakeDamage;
    public UnityEvent OnLifeLost;
    public UnityEvent OnPlayerDeath;

    [Header("Referencias UI")]
    [SerializeField] private HealthBarController healthBarController;

    private HealthSystem healthSystem;
    private Rigidbody playerRb;

    private float lastDamageTime;
    private int currentLives;

    // Propiedades
    public int CurrentLives => currentLives;
    public bool IsInvulnerable => Time.time - lastDamageTime < damageCooldown;
    public float CurrentHealthPercentage => healthSystem != null ? healthSystem.HealthPercentage : 0f;
    public bool IsDead { get; private set; } = false;

    private void Awake()
    {
        InitializeComponents();
        currentLives = maxLives;
        lastDamageTime = -1f;
    }

    private void InitializeComponents()
    {
        healthSystem = GetComponent<HealthSystem>() ?? gameObject.AddComponent<HealthSystem>();
        playerRb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        UpdateHealthBarUI();
    }

    public void TakeDamage(int damage)
    {
        if (IsDead || IsInvulnerable || healthSystem == null)
        {
            Debug.Log($"[PlayerHealth] Daño IGNORADO | IsDead: {IsDead} | Invulnerable: {IsInvulnerable}");
            return;
        }

        lastDamageTime = Time.time;

        int healthBefore = healthSystem.CurrentHealth;
        healthSystem.TakeDamage(damage);
        int healthAfter = healthSystem.CurrentHealth;

        Debug.Log($"[PlayerHealth] Daño: {damage} | Antes: {healthBefore} → Después: {healthAfter}");

        OnTakeDamage?.Invoke();
        UpdateHealthBarUI();

        if (healthAfter <= 0)
        {
            Debug.Log("→ ¡SALUD LLEGÓ A 0! → Llamando a LoseLife()");
            LoseLife();
        }
    }

    private void LoseLife()
    {
        currentLives--;
        Debug.Log($"[LoseLife] Vidas restantes: {currentLives}");

        OnLifeLost?.Invoke();

        if (currentLives <= 0)
        {
            Debug.Log("=== SIN VIDAS → MUERTE DEFINITIVA ===");
            Die();
        }
        else
        {
            Debug.Log("→ Quedan vidas → Reseteando barra");
            healthSystem.ResetHealth();
            UpdateHealthBarUI();
        }
    }

    private void Die()
    {
        IsDead = true;
        Debug.Log("=== JUGADOR HA MUERTO DEFINITIVAMENTE ===");

        OnPlayerDeath?.Invoke();

        if (playerRb != null)
        {
            playerRb.linearVelocity = Vector3.zero;
            playerRb.isKinematic = true;
        }
    }

    public void Heal(int amount)
    {
        if (IsDead || healthSystem == null) return;
        healthSystem.Heal(amount);
        UpdateHealthBarUI();
    }

    private void UpdateHealthBarUI()
    {
        if (healthBarController != null)
            healthBarController.UpdateHealthBar(healthSystem.HealthPercentage);
    }
}