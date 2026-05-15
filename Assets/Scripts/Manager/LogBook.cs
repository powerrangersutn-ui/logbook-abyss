using UnityEngine;
using UnityEngine.InputSystem;

public class Logbook : MonoBehaviour
{
    [Header("Dialog Settings")]
    public string dialogID;
    [SerializeField] private GameObject dialogElevator;

    [Header("Interaction Settings")]
    [SerializeField] private float uiShowDistance = 4f;
    [SerializeField] private float interactionDistance = 3f;

    [Header("UI")]
    [SerializeField] private GameObject interactionCanvas;

    private Transform playerTransform;
    private Camera mainCamera;

    [Header("Input Actions")]
    [SerializeField] private InputActionAsset PlayerControls;

    private InputAction useAction;

    private void Start()
    {
        mainCamera = Camera.main;

        if (interactionCanvas != null)
            interactionCanvas.SetActive(false);

        // Buscar al jugador
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;

            if (PlayerControls != null)
            {
                useAction = PlayerControls.FindActionMap("Player").FindAction("Use");
            }
            else
            {
                Debug.LogError("No se encontró componente PlayerInput en el Player");
            }
        }
        else
        {
            Debug.LogError("No se encontró ningún objeto con Tag 'Player'");
        }
    }

    private void Update()
    {
        if (playerTransform == null) return;

        float distance = Vector3.Distance(transform.position, playerTransform.position);

        // Mostrar / Ocultar Canvas según distancia
        if (distance <= uiShowDistance)
        {
            if (interactionCanvas != null && !interactionCanvas.activeSelf)
                interactionCanvas.SetActive(true);
        }
        else
        {
            if (interactionCanvas != null && interactionCanvas.activeSelf)
                interactionCanvas.SetActive(false);
        }

        // Detectar tecla F
        if (useAction != null && useAction.WasPerformedThisFrame())
        {
            Debug.Log("<color=green>¡F presionada! Intentando recoger...</color>");

            if (distance <= interactionDistance && IsLookingAtLogbook())
            {
                Debug.Log("<color=lime>¡ÉXITO! Recogiendo Logbook...</color>");
                CollectLogbook();
            }
            else
            {
                Debug.LogWarning($"F presionada - Condiciones no cumplidas → Dist: {distance:F2} | Mirando: {IsLookingAtLogbook()}");
            }
        }
    }

    private bool IsLookingAtLogbook()
    {
        if (mainCamera == null) return false;

        Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        if (Physics.Raycast(ray, out RaycastHit hit, interactionDistance + 1f))
        {
            return hit.transform == transform;
        }
        return false;
    }

    private void CollectLogbook()
    {
        Debug.Log("<color=lime>Logbook recogido exitosamente!</color>");

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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, uiShowDistance);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, interactionDistance);
    }
}