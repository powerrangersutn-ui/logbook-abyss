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
        if (GameManager.Instance != null)
        {
            GameManager.Instance.PlayerDied();
        }
    }
}