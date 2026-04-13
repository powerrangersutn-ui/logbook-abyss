using UnityEngine;
using System.Collections.Generic;

public class HarpoonPool : MonoBehaviour
{
    [Header("Configuración de Pool")]
    [SerializeField] private GameObject harpoonPrefab;
    [SerializeField] private int poolSize = 3;

    [Header("Debug")]
    [SerializeField] private bool showDebugInfo = true;

    private Queue<GameObject> availableHarpoons = new Queue<GameObject>();
    private HashSet<GameObject> activeHarpoons = new HashSet<GameObject>();
    private HarpoonGun harpoonGun;

    private void Awake()
    {
        harpoonGun = FindAnyObjectByType<HarpoonGun>();
        CreatePool();
    }

    private void CreatePool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject harpoon = Instantiate(harpoonPrefab);
            harpoon.name = $"Harpoon_{i}";
            harpoon.SetActive(false);
            availableHarpoons.Enqueue(harpoon);
        }

        if (showDebugInfo)
            Debug.Log($"Pool creado con {poolSize} arpones");
    }

    public GameObject GetHarpoon()
    {
        if (availableHarpoons.Count > 0)
        {
            GameObject harpoonObj = availableHarpoons.Dequeue();
            harpoonObj.SetActive(true);
            activeHarpoons.Add(harpoonObj);

            if (showDebugInfo)
                Debug.Log($"Arpón obtenido. Disponibles: {availableHarpoons.Count}/{poolSize}");

            return harpoonObj;
        }

        if (showDebugInfo)
            Debug.LogWarning("¡No hay arpones disponibles! Recoge los que disparaste.");

        return null;
    }

    public void ReturnToPool(GameObject harpoonObj)
    {
        if (harpoonObj == null) return;

        // Remover de activos
        activeHarpoons.Remove(harpoonObj);

        // Reset
        harpoonObj.SetActive(false);
        harpoonObj.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);

        Harpoon harpoon = harpoonObj.GetComponent<Harpoon>();
        if (harpoon != null)
        {
            harpoon.ResetForPool();
        }

        // Retornar a disponibles
        availableHarpoons.Enqueue(harpoonObj);

        if (showDebugInfo)
            Debug.Log($"Arpón recuperado. Disponibles: {availableHarpoons.Count}/{poolSize}");
    }

    public int GetAvailableCount() => availableHarpoons.Count;
    public int GetActiveCount() => activeHarpoons.Count;
    public int GetTotalCount() => poolSize;
    public HarpoonGun GetHarpoonGun() => harpoonGun;
}