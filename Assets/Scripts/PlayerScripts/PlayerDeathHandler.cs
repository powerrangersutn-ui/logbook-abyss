using UnityEngine;

public class PlayerDeathHandler : MonoBehaviour
{
    private PlayerHealth playerHealth;

    private void Awake()
    {
        playerHealth = GetComponent<PlayerHealth>();

        if (playerHealth == null)
            Debug.LogError("PlayerHealth no encontrado en PlayerCharacter", this);
    }

    private void OnEnable()
    {
        if (playerHealth != null)
            playerHealth.OnPlayerDeath.AddListener(OnPlayerDied);
    }

    private void OnDisable()
    {
        if (playerHealth != null)
            playerHealth.OnPlayerDeath.RemoveListener(OnPlayerDied);
    }

    private void OnPlayerDied()
    {
        Debug.Log("=== JUGADOR HA MUERTO - Llamando a GameManager ===");
        
        if (GameManager.Instance != null)
        {
            GameManager.Instance.PlayerDied();
        }
        else
        {
            Debug.LogError("GameManager.Instance no encontrado!");
        }
    }
}