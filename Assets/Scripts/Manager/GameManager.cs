using UnityEngine;
using TMPro;                    // Si usas TextMeshPro (recomendado)
using UnityEngine.UI;           // Si usas texto UI normal de Unity
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Pantallas de Fin de Juego")]
    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private GameObject gameOverPanel;

    [Header("Textos")]
    [SerializeField] private TextMeshProUGUI victoryText;
    [SerializeField] private TextMeshProUGUI gameOverText;

    [Header("Estado del Juego")]
    public bool hasLogbook = false;
    private bool gameEnded = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        // Asegurarnos que los paneles empiecen desactivados
        if (victoryPanel != null) victoryPanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
    }

    // ====================== VICTORIA ======================
    public void PlayerReachedExit()
    {
        if (gameEnded) return;

        if (hasLogbook)
        {
            ShowVictoryScreen();
        }
        else
        {
            Debug.LogWarning("¡Necesitas recoger la Vitácora para escapar!");
        }
    }

    private void ShowVictoryScreen()
    {
        gameEnded = true;
        Time.timeScale = 0f;                    // Pausa el juego

        if (victoryPanel != null)
        {
            victoryPanel.SetActive(true);
        }

        if (victoryText != null)
        {
            victoryText.text = "¡VICTORIA!\nEscapaste con la Vitácora";
        }

        Debug.Log("¡Victoria!");
    }

    // ====================== DERROTA ======================
    public void PlayerDied()
    {
        if (gameEnded) return;

        gameEnded = true;
        Time.timeScale = 0f;                    // Pausa el juego

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        if (gameOverText != null)
        {
            gameOverText.text = "PERDISTE";
        }

        Debug.Log("Game Over - Te quedaste sin vidas");
    }

    // ====================== VITÁCORA ======================
    public void CollectLogbook()
    {
        hasLogbook = true;
        Debug.Log("¡Vitácora recogida!");
    }

    // ====================== REINICIO (opcional) ======================
    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}