using UnityEngine;

public class Logbook : MonoBehaviour
{
    public string dialogID;
    [SerializeField] private GameObject dialogElevator;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.CollectLogbook();
                DialogManager.Instance.ShowDialog(dialogID);
                dialogElevator.SetActive(true);
            }
            Destroy(gameObject);        // Se recoge la vitácora
        }
    }
}