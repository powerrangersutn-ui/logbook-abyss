using UnityEngine;

public class PickupBox : MonoBehaviour
{
    [Header("Cantidades de Oxígeno")]
    [SerializeField] private float minOxygen = 18f;
    [SerializeField] private float maxOxygen = 35f;

    [Header("Probabilidades de Arpones")]
    [Range(0, 100)][SerializeField] private float chance1Harpoon = 40f;
    [Range(0, 100)][SerializeField] private float chance2Harpoons = 35f;
    [Range(0, 100)][SerializeField] private float chance3Harpoons = 25f;

    [Header("Probabilidad general")]
    [Range(0, 100)][SerializeField] private float harpoonChanceWhenNormal = 80f;

    private bool wasPickedUp = false;

    public void OnPickup(PlayerInventory inventory)
    {
        if (wasPickedUp) return;
        wasPickedUp = true;

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

        Destroy(gameObject, 0.3f);
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
            Debug.Log($"[PickupBox] ✅ ÉXITO dando arpones: +{given}");
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
}