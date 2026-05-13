using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections;

public class HarpoonGun : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private HarpoonPool harpoonPool;
    [SerializeField] private Transform firePoint;
    [SerializeField] private PlayerInventory playerInventory;

    [Header("Configuración")]
    [SerializeField] private float harpoonSpeed = 32f;
    [SerializeField] private float fireCooldown = 0.8f;

    [Header("Burbujas")]
    [SerializeField] private float bubbleDuration = 3f;

    [Header("Input")]
    [SerializeField] private InputActionReference fireAction;

    [Header("UI (Opcional)")]
    [SerializeField] private TextMeshProUGUI ammoText;
    [SerializeField] private bool showAmmoCount = true;

    private float nextFireTime;

    private void Awake()
    {
        if (playerInventory == null)
            playerInventory = GetComponentInParent<PlayerInventory>();
    }

    private void Update()
    {
        if (fireAction == null || harpoonPool == null || playerInventory == null) return;

        if (showAmmoCount)
            UpdateAmmoUI();

        if (fireAction.action.WasPressedThisFrame() &&
            Time.time >= nextFireTime &&
            playerInventory.HasAmmo() &&
            harpoonPool.GetAvailableCount() > 0)
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        if (firePoint == null) return;

        if (!playerInventory.HasAmmo())
        {
            Debug.Log("¡Sin munición!");
            return;
        }

        GameObject harpoonObj = harpoonPool.GetHarpoon();
        if (harpoonObj == null)
        {
            Debug.Log("No hay arpones físicos disponibles en el pool.");
            return;
        }

        if (!playerInventory.UseHarpoon())
        {
            harpoonPool.ReturnToPool(harpoonObj);
            return;
        }

        harpoonObj.transform.SetPositionAndRotation(firePoint.position, firePoint.rotation);

        Harpoon harpoon = harpoonObj.GetComponent<Harpoon>();
        if (harpoon != null)
        {
            harpoon.Initialize(harpoonPool);
            harpoon.Launch(firePoint.forward, harpoonSpeed);
        }

        // === ACTIVAR BURBUJAS AL DISPARAR ===
        ActivateBubbles(harpoonObj);

        nextFireTime = Time.time + fireCooldown;
    }

    // ==================== BURBUJAS ====================
    private void ActivateBubbles(GameObject harpoonObj)
    {
        if (harpoonObj == null)
        {
            Debug.LogError("HarpoonObj es null");
            return;
        }

        // Buscamos todas las partículas hijas
        ParticleSystem[] allBubbles = harpoonObj.GetComponentsInChildren<ParticleSystem>(true); // true = incluir inactivos

        Debug.Log($"Se encontraron {allBubbles.Length} ParticleSystems en el arpón.");

        if (allBubbles.Length > 0)
        {
            ParticleSystem bubbles = allBubbles[0];   // tomamos la primera
            Debug.Log("Activando burbujas: " + bubbles.gameObject.name);

            bubbles.gameObject.SetActive(true);   // Aseguramos que el GameObject esté activo
            bubbles.Play();

            StartCoroutine(StopBubblesAfterTime(bubbles, bubbleDuration));
        }
        else
        {
            Debug.LogError("¡NO SE ENCONTRARON BURBUJAS en el arpón! Revisa la jerarquía del prefab.");
        }
    }

    private IEnumerator StopBubblesAfterTime(ParticleSystem ps, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (ps != null)
            ps.Stop();
    }

    // ==================== Resto de tus métodos ====================
    private void UpdateAmmoUI()
    {
        if (ammoText != null && playerInventory != null)
        {
            int current = playerInventory.CurrentHarpoons;
            int max = playerInventory.MaxHarpoons;
            ammoText.text = $"{current}/{max}";

            if (current == 0)
                ammoText.color = Color.red;
            else if (current == 1)
                ammoText.color = Color.yellow;
            else
                ammoText.color = Color.white;
        }
    }

    public void CollectHarpoon(GameObject harpoonObject)
    {
        if (harpoonPool != null)
            harpoonPool.ReturnToPool(harpoonObject);
    }

    private void OnEnable()
    {
        if (fireAction != null) fireAction.action.Enable();
    }

    private void OnDisable()
    {
        if (fireAction != null) fireAction.action.Disable();
    }
}