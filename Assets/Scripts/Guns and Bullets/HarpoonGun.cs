using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class HarpoonGun : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private HarpoonPool harpoonPool;
    [SerializeField] private Transform firePoint;

    [Header("Configuración")]
    [SerializeField] private float harpoonSpeed = 32f;
    [SerializeField] private float fireCooldown = 0.8f;

    [Header("Input")]
    [SerializeField] private InputActionReference fireAction;

    [Header("UI (Opcional)")]
    [SerializeField] private TextMeshProUGUI ammoText;
    [SerializeField] private bool showAmmoCount = true;

    private float nextFireTime;

    private void Update()
    {
        if (fireAction == null || harpoonPool == null) return;

        // Actualizar UI
        if (showAmmoCount)
            UpdateAmmoUI();

        // Disparar
        if (fireAction.action.WasPressedThisFrame() &&
            Time.time >= nextFireTime &&
            harpoonPool.GetAvailableCount() > 0)
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        if (firePoint == null) return;

        GameObject harpoonObj = harpoonPool.GetHarpoon();
        if (harpoonObj == null)
        {
            Debug.Log("No hay arpones disponibles. ¡Recoge los que disparaste!");
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
        if (ammoText != null)
        {
            int available = harpoonPool.GetAvailableCount();
            int total = harpoonPool.GetTotalCount();
            ammoText.text = $"{available}/{total}";

            // Cambiar color si quedan pocos
            if (available == 0)
                ammoText.color = Color.red;
            else if (available == 1)
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