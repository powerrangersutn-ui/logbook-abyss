using UnityEngine;

//Es el Mediador (de lógica). Separa la muerte del jugador de la lógica global que está en el GameManager
public class PlayerDeathHandler : MonoBehaviour
{
    // Ya no necesitamos la referencia privada a PlayerHealth porque escuchamos el evento global.

    private void OnEnable()
    {
        UIGameEvents.onPlayerDeath += OnPlayerDied;
    }

    private void OnDisable()
    {
        UIGameEvents.onPlayerDeath -= OnPlayerDied;
    }

    private void OnPlayerDied()
    {
        Debug.Log("=== [DeathHandler] El evento de muerte fue captado ===");

        if (GameManager.Instance != null)
        {
            GameManager.Instance.PlayerDied();
        }
        else
        {
            Debug.LogWarning("GameManager.Instance no encontrado. Asegúrate de que exista uno en la escena.");
        }
    }
}