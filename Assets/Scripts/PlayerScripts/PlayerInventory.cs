using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [Header("Arpones")]
    [SerializeField] private int maxHarpoons = 3;
    [SerializeField] private int currentHarpoons = 0;

    [Header("Referencias")]
    [SerializeField] private HarpoonPool harpoonPool;

    private OxygenSystem oxygenSystem;

    public int CurrentHarpoons => currentHarpoons;
    public int MaxHarpoons => maxHarpoons;

    private void Awake()
    {
        oxygenSystem = GetComponent<OxygenSystem>();

        if (harpoonPool == null)
            harpoonPool = FindAnyObjectByType<HarpoonPool>();

        Debug.Log($"[Inventory] Inicializado - Arpones: {currentHarpoons}/{maxHarpoons} | Pool: {(harpoonPool != null ? "OK" : "NULL")}");
    }

    public void AddOxygen(float amount)
    {
        oxygenSystem?.AddOxygen(amount);
    }

    public int AddHarpoons(int desiredAmount)
    {
        Debug.Log($"[Inventory] AddHarpoons llamado  Quieren dar: {desiredAmount}");

        int spaceAvailable = maxHarpoons - currentHarpoons;
        if (spaceAvailable <= 0)
        {
            Debug.LogWarning("[Inventory] No hay espacio para más arpones");
            return 0;
        }

        int amountToGive = Mathf.Min(desiredAmount, spaceAvailable);

        // Si hay pool, intentamos usar de ahí
        if (harpoonPool != null)
        {
            int available = harpoonPool.GetAvailableCount();
            Debug.Log($"[Inventory] Arpones disponibles en Pool: {available}");
            // Por ahora no limitamos por pool (como pediste)
        }

        currentHarpoons += amountToGive;

        Debug.Log($"[Inventory] ✅ ¡Arpones añadidos! +{amountToGive} → Total: {currentHarpoons}/{maxHarpoons}");
        return amountToGive;
    }

    public bool CanReceiveHarpoons()
    {
        bool can = currentHarpoons < maxHarpoons;
        Debug.Log($"[Inventory] CanReceiveHarpoons() = {can} (actual: {currentHarpoons}/{maxHarpoons})");
        return can;
    }
}