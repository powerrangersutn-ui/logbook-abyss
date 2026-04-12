using UnityEngine;
using System.Collections.Generic;

public class HarpoonPool : MonoBehaviour
{
    [SerializeField] private GameObject harpoonPrefab;
    [SerializeField] private int poolSize = 3;

    private Queue<GameObject> pool = new Queue<GameObject>();
    private HarpoonGun harpoonGun;   // Referencia directa

    private void Awake()
    {
        harpoonGun = FindAnyObjectByType<HarpoonGun>(); // Solo se busca una vez al inicio
        CreatePool();
    }

    private void CreatePool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject harpoon = Instantiate(harpoonPrefab);
            harpoon.SetActive(false);
            pool.Enqueue(harpoon);
        }
    }

    public GameObject GetHarpoon()
    {
        if (pool.Count > 0)
        {
            GameObject harpoon = pool.Dequeue();
            harpoon.SetActive(true);
            return harpoon;
        }
        return null;
    }

    public void ReturnToPool(GameObject harpoon)
    {
        if (harpoon == null) return;

        harpoon.SetActive(false);
        harpoon.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);

        Harpoon h = harpoon.GetComponent<Harpoon>();
        if (h != null)
            h.ResetHarpoon();

        pool.Enqueue(harpoon);
    }

    public int GetAvailableCount() => pool.Count;

    // Getter para que Harpoon pueda devolver el arpón sin FindAnyObjectByType
    public HarpoonGun GetHarpoonGun() => harpoonGun;
}