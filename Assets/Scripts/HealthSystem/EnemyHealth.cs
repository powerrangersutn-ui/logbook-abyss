using UnityEngine;
using System.Collections.Generic;

public class EnemyHealth : MonoBehaviour
{
    [Header("Salud del Enemigo")]
    [SerializeField] private int maxHealth = 3;

    [Header("Debug Visual")]
    [SerializeField] private bool showStuckHarpoons = true;

    [Header("Audio")]
    [SerializeField] private AudioClip deathSound;
    [SerializeField][Range(0f, 1f)] private float soundVolume = 0.5f;

    private HealthSystem healthSystem;
    private List<Harpoon> stuckHarpoons = new List<Harpoon>();
    private bool isDead = false;

    private void Awake()
    {
        healthSystem = GetComponent<HealthSystem>() ?? gameObject.AddComponent<HealthSystem>();
        healthSystem.SetMaxHealth(maxHealth);
    }

    private void OnEnable()
    {
        healthSystem.OnDeath += HandleDeath;
    }

    private void OnDisable()
    {
        healthSystem.OnDeath -= HandleDeath;
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;
        healthSystem?.TakeDamage(damage);
    }

    // ← NUEVO: Método público para que el arpón pueda verificar
    public int GetCurrentHealth()
    {
        return healthSystem != null ? healthSystem.CurrentHealth : 0;
    }

    public void OnHarpoonStuck(Harpoon harpoon)
    {
        if (isDead) return;

        if (!stuckHarpoons.Contains(harpoon))
        {
            stuckHarpoons.Add(harpoon);

            if (showStuckHarpoons)
                Debug.Log($"{gameObject.name} tiene {stuckHarpoons.Count} arpones clavados. Vida: {healthSystem.CurrentHealth}/{healthSystem.MaxHealth}");
        }
    }

    public void OnHarpoonRemoved(Harpoon harpoon)
    {
        stuckHarpoons.Remove(harpoon);
    }

    private void HandleDeath()
    {
        if (isDead) return;

        AudioSource.PlayClipAtPoint(deathSound, transform.position, soundVolume);

        isDead = true;

        if (showStuckHarpoons)
            Debug.Log($"{gameObject.name} murió. Soltando {stuckHarpoons.Count} arpones.");

        ReleaseAllStuckHarpoons();
        Destroy(gameObject);
    }

    private void ReleaseAllStuckHarpoons()
    {
        List<Harpoon> harpoonsToRelease = new List<Harpoon>(stuckHarpoons);

        foreach (Harpoon harpoon in harpoonsToRelease)
        {
            if (harpoon != null && harpoon.gameObject != null)
            {
                harpoon.transform.SetParent(null);
                harpoon.ReleaseFromEnemy();
            }
        }

        stuckHarpoons.Clear();
    }

    public int GetStuckHarpoonCount() => stuckHarpoons.Count;

    private void OnDestroy()
    {
        if (!isDead && stuckHarpoons.Count > 0)
        {
            Debug.LogWarning($"{gameObject.name} fue destruido sin soltar arpones. Liberándolos ahora.");
            ReleaseAllStuckHarpoons();
        }
    }
}