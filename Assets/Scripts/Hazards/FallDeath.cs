using UnityEngine;

public class FallDeath : MonoBehaviour
{
    [SerializeField] private GameObject gameOverPanel;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")|| other.CompareTag("Respawn"))
        {
            gameOverPanel.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Time.timeScale = 0f;
        }
    }
}
