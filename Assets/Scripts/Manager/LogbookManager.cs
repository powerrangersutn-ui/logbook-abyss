using UnityEngine;

public class LogbookManager : MonoBehaviour, IInteractable
{
    [Header("Settings")]
    public string dialogID;
    public string messageKey = "logbook";
    [SerializeField] private GameObject dialogElevator;

    [Header("Sounds")]
    [SerializeField] private AudioClip pickupSound;

    private bool wasPickedUp = false;


    public void OnInteract(PlayerInventory inventory)
    {
        if (wasPickedUp) return;
        wasPickedUp = true;

        DiaryManager.RaiseLogbookFound();
        GameManager.Instance.hasLogbook = wasPickedUp;

        //Muestra mensaje de que se encontró la bitácora
        DialogManager.Instance.ShowDialog(dialogID);

        if (dialogElevator != null)
            dialogElevator.SetActive(true);

        GetComponent<LogbookPickup>()?.MarkAsCollected();
        AudioSource.PlayClipAtPoint(pickupSound, transform.position);

        Destroy(gameObject);
    }
}