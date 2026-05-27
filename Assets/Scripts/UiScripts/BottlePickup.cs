using UnityEngine;

public class BottlePickup : MonoBehaviour, IInteractable
{
    private bool wasPickedUp = false;

    [SerializeField] private float uiShowDistance = 3f;
    [SerializeField] private GameObject interactionCanvas;

    private Transform playerTransform;

    [Header("Ruidos")]
    private AudioSource m_audiosource;
    [SerializeField] private AudioClip pickupSound;

    private void Start()
    {
        m_audiosource = GetComponent<AudioSource>();
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) playerTransform = player.transform;
    }

    private void Update()
    {
        if (wasPickedUp || interactionCanvas == null)
        {
            if (interactionCanvas != null) interactionCanvas.SetActive(false);
            return;
        }

        if (playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) playerTransform = player.transform;
            return;
        }

        float distance = Vector3.Distance(transform.position, playerTransform.position);
        interactionCanvas.SetActive(distance <= uiShowDistance);
    }

    public void OnInteract(PlayerInventory inventory)
    {
        if (wasPickedUp) return;
        wasPickedUp = true;

        if (interactionCanvas != null) interactionCanvas.SetActive(false);

        DiaryManager.RaiseBottleFound();

        m_audiosource.PlayOneShot(pickupSound);
        Destroy(gameObject, 0.5f);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, uiShowDistance);
    }
}