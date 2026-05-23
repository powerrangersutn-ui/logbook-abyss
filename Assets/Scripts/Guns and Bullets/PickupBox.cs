using UnityEngine;

public class PickupBox : MonoBehaviour
{
    [Header("Cantidades de Oxígeno")]
    [SerializeField] private float minOxygen = 18f;
    [SerializeField] private float maxOxygen = 35f;

    [Header("Probabilidades de Arpones")]
    [Range(0, 100)][SerializeField] private float chance1Harpoon = 40f;
    [Range(0, 100)][SerializeField] private float chance2Harpoons = 35f;

    [Header("Probabilidad general")]
    [Range(0, 100)][SerializeField] private float harpoonChanceWhenNormal = 80f;

    [Header("Interaction Settings")]
    [SerializeField] private float uiShowDistance = 4f;

    [Header("UI")]
    [SerializeField] private GameObject interactionCanvas;

    private bool wasPickedUp = false;

    // Método que llamará PlayerControl
    public void Interact()
    {
        OnPickup();
    }

    private void OnPickup()
    {
        if (wasPickedUp) return;
        wasPickedUp = true;

        // Buscar el jugador y su inventario
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("[PickupBox] No se encontró al jugador!");
            return;
        }

        PlayerInventory inventory = player.GetComponent<PlayerInventory>();
        if (inventory == null)
        {
            Debug.LogError("[PickupBox] El jugador no tiene PlayerInventory!");
            return;
        }

        Debug.Log($"[PickupBox] Caja recogida! Oxígeno bajo = {inventory.GetComponent<OxygenSystem>().IsLowOxygen}");

        if (inventory.GetComponent<OxygenSystem>().IsLowOxygen)
        {
            GiveOxygen(inventory);
        }
        else
        {
            if (Random.value * 100f <= harpoonChanceWhenNormal && inventory.CanReceiveHarpoons())
            {
                Debug.Log("[PickupBox] Decidió dar ARPONES");
                GiveHarpoons(inventory);
            }
            else
            {
                Debug.Log("[PickupBox] Decidió dar OXÍGENO");
                GiveOxygen(inventory);
            }
        }

        if (interactionCanvas != null)
            interactionCanvas.SetActive(false);

        Destroy(gameObject, 0.3f);
    }

    private void Update()
    {
        if (interactionCanvas == null || wasPickedUp) return;

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

    private void GiveOxygen(PlayerInventory inventory)
    {
        float amount = Random.Range(minOxygen, maxOxygen);
        inventory.AddOxygen(amount);
        Debug.Log($"[PickupBox] Oxígeno dado: +{amount:F1}");
    }

    private void GiveHarpoons(PlayerInventory inventory)
    {
        int amount = GetHarpoonAmountByProbability();
        Debug.Log($"[PickupBox] Intentando dar {amount} arpones");
        int given = inventory.AddHarpoons(amount);
        if (given > 0)
            Debug.Log($"[PickupBox] dando arpones: +{given}");
        else
            Debug.LogWarning("[PickupBox] No se dieron arpones");
    }

    private int GetHarpoonAmountByProbability()
    {
        float roll = Random.value * 100f;
        float cumulative = chance1Harpoon;
        if (roll <= cumulative) return 1;
        cumulative += chance2Harpoons;
        if (roll <= cumulative) return 2;
        return 3;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, uiShowDistance);
    }
}