using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [Header("Munición de Arpones")]
    [SerializeField] private int maxHarpoons = 3;
    [SerializeField] private int currentHarpoons = 3;

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

    }

    public void AddOxygen(float amount)
    {
        oxygenSystem?.AddOxygen(amount);
    }

    public int AddHarpoons(int desiredAmount)
    {
        int spaceAvailable = maxHarpoons - currentHarpoons;

        if (spaceAvailable <= 0)
        {
            return 0;
        }

        int toAdd = Mathf.Min(desiredAmount, spaceAvailable);
        currentHarpoons += toAdd;

        return toAdd;
    }


    public bool CanReceiveHarpoons()
    {
        bool can = currentHarpoons < maxHarpoons;
        return can;
    }


    public bool UseHarpoon()
    {
        if (currentHarpoons <= 0)
        {
            return false;
        }

        currentHarpoons--;
        return true;
    }

    public bool HasAmmo()
    {
        return currentHarpoons > 0;
    }
}