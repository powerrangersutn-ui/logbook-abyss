using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.Audio;

public class PausePanelController : MonoBehaviour
{

    [Header("Configuración")]
    [SerializeField] private string mainMenuScene;

    [Header("Panel de Pausa")]
    [SerializeField] private GameObject pausePanelMenu;

    [Header("Input Actions")]
    [SerializeField] private InputActionAsset UIControls;

    [Header("AudioMixer Snapshots")]
    [SerializeField] private AudioMixerSnapshot snapshotDefault;
    [SerializeField] private AudioMixerSnapshot snapshotPause;


    private InputAction pauseAction;

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

    private void Awake()
    {
        pauseAction = UIControls.FindActionMap("UI").FindAction("Pause");
    }

    private void OnEnable()
    {
        pauseAction.Enable();
    }

    private void OnDisable()
    {
        pauseAction.Disable();
    }

    void Update()
    {
        if (pauseAction.triggered)
        {
            if (pausedGame)
            {
                Unpause();
            }
            else
            {
                Pause();
            }
            ChangePausedAudio(pausedGame);
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

    public void ChangePausedAudio(bool isPaused)
    {
        // El número adentro de TransitionTo es el tiempo en segundos que dura el "fade"
        float transitionTime = 0f;

        if (isPaused)
        {
            // Activa el efecto de pausa (ej: volumen bajo, filtro de ecualizador)
            snapshotPause.TransitionTo(transitionTime);
        }
        else
        {
            // Vuelve a la mezcla de sonido normal
            snapshotDefault.TransitionTo(transitionTime);
        }
    }

}

