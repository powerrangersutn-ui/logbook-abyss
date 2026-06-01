using UnityEngine;

public class LogbookManager : MonoBehaviour, IInteractable
{
    [Header("Settings")]
    public string dialogID;
    public string messageKey = "logbook";
    [SerializeField] private GameObject dialogElevator;

    [Header("Sounds")]
    private AudioSource m_audiosource;
    [SerializeField] private AudioClip pickupSound;

    private bool wasPickedUp = false;

    private void Start()
    {
        m_audiosource = GetComponent<AudioSource>();
    }

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
        m_audiosource.PlayOneShot(pickupSound);

        Destroy(gameObject, 0.5f);
    }
}