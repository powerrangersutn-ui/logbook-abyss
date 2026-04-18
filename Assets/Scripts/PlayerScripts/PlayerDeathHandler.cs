using UnityEngine;

public class PlayerDeathHandler : MonoBehaviour
{
    [Header("Configuraci�n de Muerte")]
    [SerializeField] private GameObject gameOverPanel;

    [Header("Opciones")]
    [SerializeField] private bool disableMovementOnDeath = true;

    private PlayerHealth playerHealth;
    private PlayerControl playerControl;           // Cambia el nombre si tu script de movimiento se llama diferente
    private CharacterController characterController;
    private Rigidbody playerRb;

    private void Awake()
    {
        playerHealth = GetComponent<PlayerHealth>();

        playerControl = GetComponent<PlayerControl>();
        characterController = GetComponent<CharacterController>();
        playerRb = GetComponent<Rigidbody>();

        if (playerHealth == null)
            Debug.LogError("PlayerHealth no encontrado en PlayerCharacter", this);

        if (gameOverPanel == null)
            Debug.LogWarning("GameOverPanel no est� asignado en el Inspector", this);
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
        Debug.Log("=== JUGADOR HA MUERTO - Activando Game Over ===");

        // Activar panel de Game Over
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            Debug.Log("Game Over Panel activado");
        }
        else
        {
            Debug.LogError("GameOverPanel no asignado en PlayerDeathHandler");
        }

        // Desactivar movimiento
        if (disableMovementOnDeath)
        {
            DisablePlayerMovement();
        }
    }

    private void DisablePlayerMovement()
    {
        Debug.Log("Desactivando movimiento del jugador...");

        // Desactivar script de movimiento
        if (playerControl != null)
            playerControl.enabled = false;

        // Desactivar CharacterController (el m�s com�n en tu proyecto)
        if (characterController != null)
            characterController.enabled = false;

        // Desactivar Rigidbody
        if (playerRb != null)
        {
            playerRb.linearVelocity = Vector3.zero;
            playerRb.isKinematic = true;
        }

        Debug.Log("Movimiento del jugador DESACTIVADO correctamente");
    }

    // Para usar desde botones del Game Over
    public void RestartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
}