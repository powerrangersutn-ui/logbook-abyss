using UnityEngine;
using UnityEngine.InputSystem;

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

    private float nextFireTime;

    private void Update()
    {
        if (fireAction == null || harpoonPool == null) return;

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
        if (harpoonObj == null) return;

        harpoonObj.transform.SetPositionAndRotation(firePoint.position, firePoint.rotation);

        Harpoon harpoon = harpoonObj.GetComponent<Harpoon>();
        if (harpoon != null)
        {
            harpoon.Initialize(harpoonPool);   // ← Importante: pasamos la referencia
            harpoon.Launch(firePoint.forward, harpoonSpeed);
        }

        nextFireTime = Time.time + fireCooldown;
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