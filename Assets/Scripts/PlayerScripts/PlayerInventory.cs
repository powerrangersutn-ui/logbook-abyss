using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [Header("Munición de Arpones")]
    [SerializeField] private int maxHarpoons = 3;
    [SerializeField] private int currentHarpoons = 3;

    [Header("Referencias")]
    [SerializeField] private HarpoonPool harpoonPool;

    private OxygenSystem oxygenSystem;

    // Propiedades públicas
    public int CurrentHarpoons => currentHarpoons;
    public int MaxHarpoons => maxHarpoons;

    private void Awake()
    {
        oxygenSystem = GetComponent<OxygenSystem>();

        if (harpoonPool == null)
            harpoonPool = FindAnyObjectByType<HarpoonPool>();

        Debug.Log($"[PlayerInventory] Inicializado - Munición: {currentHarpoons}/{maxHarpoons}");
    }

    public void AddOxygen(float amount)
    {
        oxygenSystem?.AddOxygen(amount);
    }

    /// <summary>
    /// Agrega munición de arpones al inventario (pickups)
    /// </summary>
    public int AddHarpoons(int desiredAmount)
    {
        int spaceAvailable = maxHarpoons - currentHarpoons;

        if (spaceAvailable <= 0)
        {
            Debug.LogWarning("[PlayerInventory] Munición llena, no se pueden agregar más arpones");
            return 0;
        }

        int toAdd = Mathf.Min(desiredAmount, spaceAvailable);
        currentHarpoons += toAdd;

        Debug.Log($"[PlayerInventory] +{toAdd} arpones! Munición: {currentHarpoons}/{maxHarpoons}");

        return toAdd;
    }

    /// Verifica si puede recibir más munición
    public bool CanReceiveHarpoons()
    {
        bool can = currentHarpoons < maxHarpoons;
        Debug.Log($"[PlayerInventory] CanReceiveHarpoons() = {can} (munición: {currentHarpoons}/{maxHarpoons})");
        return can;
    }

    /// Usa un arpón al disparar. Llama esto desde HarpoonGun.
    public bool UseHarpoon()
    {
        if (currentHarpoons <= 0)
        {
            Debug.LogWarning("[PlayerInventory] Sin munición!");
            return false;
        }

        currentHarpoons--;
        Debug.Log($"[PlayerInventory] Arpón disparado. Munición restante: {currentHarpoons}/{maxHarpoons}");
        return true;
    }

    /// <summary>
    /// Verifica si tiene munición disponible
    /// </summary>
    public bool HasAmmo()
    {
        return currentHarpoons > 0;
    }
}