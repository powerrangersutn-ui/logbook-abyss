using UnityEngine;

public class LogbookManager : MonoBehaviour, IInteractable
{
    [Header("Settings")]
    public string dialogID;
    public string messageKey = "logbook";
    [SerializeField] private GameObject dialogElevator;

    private bool wasPickedUp = false;

    public void OnInteract(PlayerInventory inventory)
    {
        if (wasPickedUp) return;
        wasPickedUp = true;

        DiaryManager.RaiseLogbookFound();

        //Muestra mensaje de que se encontró la bitácora
        DialogManager.Instance.ShowDialog(dialogID);

        if (dialogElevator != null)
            dialogElevator.SetActive(true);

        GetComponent<LogbookPickup>()?.MarkAsCollected();

        Destroy(gameObject, 0.1f);
    }
}