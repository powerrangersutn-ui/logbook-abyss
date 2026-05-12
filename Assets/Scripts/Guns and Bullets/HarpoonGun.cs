using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class HarpoonGun : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private HarpoonPool harpoonPool;
    [SerializeField] private Transform firePoint;
    [SerializeField] private PlayerInventory playerInventory; // NUEVO

    [Header("Configuración")]
    [SerializeField] private float harpoonSpeed = 32f;
    [SerializeField] private float fireCooldown = 0.8f;

    [Header("Input")]
    [SerializeField] private InputActionReference fireAction;

    [Header("UI (Opcional)")]
    [SerializeField] private TextMeshProUGUI ammoText;
    [SerializeField] private bool showAmmoCount = true;

    private float nextFireTime;

    private void Awake()
    {
        // Auto-asignar PlayerInventory si no está configurado
        if (playerInventory == null)
            playerInventory = GetComponentInParent<PlayerInventory>();
    }

    private void Update()
    {
        if (fireAction == null || harpoonPool == null || playerInventory == null) return;

        // Actualizar UI
        if (showAmmoCount)
            UpdateAmmoUI();

        // Disparar - AHORA verifica munición en lugar de pool
        if (fireAction.action.WasPressedThisFrame() &&
            Time.time >= nextFireTime &&
            playerInventory.HasAmmo() && // CAMBIADO: verifica munición
            harpoonPool.GetAvailableCount() > 0) // Pool debe tener arpones físicos
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        if (firePoint == null) return;

        // Verificar munición
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

        // GASTAR MUNICIÓN
        if (!playerInventory.UseHarpoon())
        {
            // Si falló, devolver el arpón al pool
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

        nextFireTime = Time.time + fireCooldown;
    }

    private void UpdateAmmoUI()
    {
        if (ammoText != null && playerInventory != null)
        {
            // CAMBIADO: muestra munición en lugar de pool disponible
            int current = playerInventory.CurrentHarpoons;
            int max = playerInventory.MaxHarpoons;
            ammoText.text = $"{current}/{max}";

            // Cambiar color si quedan pocos
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