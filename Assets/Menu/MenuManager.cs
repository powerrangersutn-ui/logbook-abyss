using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private string primeraEscena = "1day";

    public void StartNewGame(string firstSceneName)
    {
        SceneManager.LoadScene(firstSceneName);
    }

    public void OnSalirClick()
    {
        Debug.Log("Saliendo del juego...");
        Application.Quit(); // Nota: Application.Quit() no hace nada dentro del editor de Unity, solo funciona en la build final
    }

}
