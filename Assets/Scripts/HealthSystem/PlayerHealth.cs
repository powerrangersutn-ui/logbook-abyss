using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : MonoBehaviour
{
    [Header("Sistema de Vidas")]
    [SerializeField] private int maxLives = 3;
    [Header("Daño por Contacto")]
    [SerializeField] private int contactDamage = 1;
    [SerializeField] private float damageCooldown = 0.8f;
    [SerializeField] private float knockbackForce = 5f;
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
        lastDamageTime = -damageCooldown;
    }

    private void InitializeComponents()
    {
        healthSystem = GetComponent<HealthSystem>();
        if (healthSystem == null)
            healthSystem = gameObject.AddComponent<HealthSystem>();
        playerRb = GetComponent<Rigidbody>();
    }

    #region Daño por Contacto
    private void OnCollisionEnter(Collision collision)
    {
        if (IsInvulnerable) return;
        if (collision.gameObject.CompareTag("Enemy"))
        {
            TakeContactDamage(collision);
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (IsInvulnerable) return;
        if (collision.gameObject.CompareTag("Enemy"))
        {
            TakeContactDamage(collision);
        }
    }

    private void TakeContactDamage(Collision collision)
    {
        TakeDamage(contactDamage);
        ApplyKnockback(collision.contacts[0].normal);
    }

    private void ApplyKnockback(Vector3 contactNormal)
    {
        if (playerRb != null)
        {
            Vector3 knockbackDirection = -contactNormal;
            knockbackDirection.y = 0.3f; // Pequeño impulso hacia arriba
            playerRb.AddForce(knockbackDirection.normalized * knockbackForce, ForceMode.Impulse);
        }
    }
    #endregion

    #region Daño General
    public void TakeDamage(int damage)
    {
        if (IsInvulnerable || !healthSystem.IsAlive) return;
        lastDamageTime = Time.time;
        healthSystem.TakeDamage(damage);
        OnTakeDamage?.Invoke();
        if (healthSystem.CurrentHealth <= 0)
        {
            LoseLife();
        }
    }

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

        //  Conexión con GameManager
        if (GameManager.Instance != null)
        {
            GameManager.Instance.PlayerDied();
        }
        else
        {
            // Fallback por si el GameManager no está en la escena
            gameObject.SetActive(false);
        }
    }
    #endregion

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