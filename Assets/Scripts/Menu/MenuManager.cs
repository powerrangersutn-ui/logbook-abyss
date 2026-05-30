using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{

    public void StartNewGame(string firstSceneName)
    {
        SceneManager.LoadScene(firstSceneName);
        Time.timeScale = 0f;
    }

    public void OnSalirClick()
    {
        Debug.Log("Saliendo del juego...");
        Application.Quit(); // Nota: Application.Quit() no hace nada dentro del editor de Unity, solo funciona en la build final
    }

}
