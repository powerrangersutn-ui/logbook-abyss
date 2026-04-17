using System.Net.NetworkInformation;
using UnityEngine;

public class FallDeath : MonoBehaviour
{
    [SerializeField] private GameObject gameOverPanel;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gameOverPanel.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
