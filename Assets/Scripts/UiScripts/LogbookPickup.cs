using UnityEngine;

public class LogbookPickup : MonoBehaviour
{
    [SerializeField] private float uiShowDistance = 3f;
    [SerializeField] private GameObject interactionCanvas;

    private Transform playerTransform;

    private bool isCollected = false;


    private void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) playerTransform = player.transform;
    }

    private void Update()
    {
        if (isCollected || interactionCanvas == null)
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, uiShowDistance);
    }

    public void MarkAsCollected()
    {
        isCollected = true;
        if (interactionCanvas != null) interactionCanvas.SetActive(false);
    }
}