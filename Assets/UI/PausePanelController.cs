using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
public class PausePanelController : MonoBehaviour
{

    [Header("Configuración")]
    [SerializeField] private string mainMenuScene;

    [Header("Panel de Pausa")]
    [SerializeField] private GameObject pausePanelMenu;

    public void GoToMenu(string mainMenuScene)
    {
        SceneManager.LoadScene(mainMenuScene);
    }

    private bool pausedGame = false;

    void Start()
    {
        // Por seguridad, nos aseguramos de que el menú esté apagado al empezar el nivel
        if (pausePanelMenu != null)
        {
            pausePanelMenu.SetActive(false);
        }
    }

    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (pausedGame)
            {
                Unpause();
            }
            else
            {
                Pause();
            }
        }
    }


    public void Pause()
    {
        if (pausePanelMenu != null) pausePanelMenu.SetActive(true);
        Time.timeScale = 0f;
        pausedGame = true;
    }


    public void Unpause()
    {
        if (pausePanelMenu != null) pausePanelMenu.SetActive(false);
        Time.timeScale = 1f;
        pausedGame = false;

        if (pausePanelMenu != null)
        {
            pausePanelMenu.SetActive(false);
        }
    }

}
