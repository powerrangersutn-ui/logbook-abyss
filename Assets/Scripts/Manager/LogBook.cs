using UnityEngine;

public class Logbook : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.CollectLogbook();
            }
            Destroy(gameObject);        // Se recoge la vitácora
        }
    }
}