using UnityEngine;
using UnityEngine.InputSystem;

public class Logbook : MonoBehaviour
{
    [Header("Dialog Settings")]
    public string dialogID;
    [SerializeField] private GameObject dialogElevator;

    [Header("Interaction Settings")]
    [SerializeField] private float uiShowDistance = 4f;

    [Header("UI")]
    [SerializeField] private GameObject interactionCanvas;

    private void CollectLogbook()
    {
        Debug.Log("Logbook recogido exitosamente!");
        if (GameManager.Instance != null)
        {
            GameManager.Instance.CollectLogbook();
            if (DialogManager.Instance != null)
                DialogManager.Instance.ShowDialog(dialogID);
        }
        if (dialogElevator != null)
            dialogElevator.SetActive(true);

        if (interactionCanvas != null)
            interactionCanvas.SetActive(false);

        Destroy(gameObject);
    }

    // Método que llamará PlayerControl
    public void Interact()
    {
        CollectLogbook();
    }

    // Mantengo esto solo para mostrar el canvas de interacción
    private void Update()
    {
        if (interactionCanvas == null) return;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.transform.position);

        if (distance <= uiShowDistance)
        {
            if (!interactionCanvas.activeSelf)
                interactionCanvas.SetActive(true);
        }
        else
        {
            if (interactionCanvas.activeSelf)
                interactionCanvas.SetActive(false);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, uiShowDistance);
    }
}