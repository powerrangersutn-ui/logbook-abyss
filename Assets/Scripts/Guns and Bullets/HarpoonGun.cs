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
    [SerializeField] private InputActionReference moveAction;

    [Header("UI (Opcional)")]
    [SerializeField] private TextMeshProUGUI ammoText;
    [SerializeField] private bool showAmmoCount = true;

    [Header("Animación de Arma")]
    [SerializeField] private Transform weaponModel;

    [Header("Balanceo al Caminar (Submarino)")]
    [SerializeField] private bool enableWalkBobbing = true;
    [SerializeField] private float bobbingSpeed = 6f;
    [SerializeField] private float bobbingAmountHorizontal = 0.08f;
    [SerializeField] private float bobbingTiltAngle = 2f;
    [SerializeField] private float bobbingLerpSpeed = 10f;

    [Header("Retroceso al Disparar")]
    [SerializeField] private bool enableRecoil = true;
    [SerializeField] private float recoilRotation = 15f;
    [SerializeField] private float recoilDownSpeed = 8f;

    [Header ("sounds")]
    [SerializeField] private AudioClip shootSound;
    [SerializeField] private AudioSource source;

    private float nextFireTime;

    // Variables para el balanceo
    private Vector3 weaponOriginalPos;
    private float walkTimer;
    private bool isWalking;

    // Variables para el retroceso
    private Quaternion weaponOriginalRot;
    private bool isRecoiling;

    private void Awake()
    {
        if (playerInventory == null)
            playerInventory = GetComponentInParent<PlayerInventory>();

        // Guardar posición y rotación original del arma
        if (weaponModel != null)
        {
            weaponOriginalPos = weaponModel.localPosition;
            weaponOriginalRot = weaponModel.localRotation;
        }
    }

    private void Update()
    {
        if (fireAction == null || harpoonPool == null || playerInventory == null || Time.timeScale!=1) return;

        if (showAmmoCount)
            UpdateAmmoUI();

        // Detectar si está caminando
        DetectWalking();

        // Aplicar balanceo si está caminando
        if (enableWalkBobbing && isWalking && !isRecoiling)
            ApplyWalkBobbing();
        else if (!isRecoiling)
            ResetWeaponPosition();

        // Disparar
        if (fireAction.action.WasPressedThisFrame() &&
            Time.time >= nextFireTime &&
            playerInventory.HasAmmo() &&
            harpoonPool.GetAvailableCount() > 0)
        {
            Shoot();

        }
        Debug.Log($"isWalking: {isWalking} | isRecoiling: {isRecoiling} | enableWalkBobbing: {enableWalkBobbing}");
    }

    // ==================== BALANCEO AL CAMINAR (VERSIÓN SUBMARINA) ====================
    private void DetectWalking()
    {
        if (moveAction == null)
        {
            isWalking = false;
            return;
        }

        Vector2 moveInput = moveAction.action.ReadValue<Vector2>();
        isWalking = moveInput.magnitude > 0.1f;
    }

    private void ApplyWalkBobbing()
    {
        if (weaponModel == null) return;

        walkTimer += Time.deltaTime * bobbingSpeed;

        // Movimiento SOLO lateral (como un péndulo bajo el agua)
        float horizontalOffset = Mathf.Sin(walkTimer) * bobbingAmountHorizontal;

        // Rotación sutil en Z para simular el balanceo del arma
        float tiltAngle = Mathf.Sin(walkTimer) * bobbingTiltAngle;

        Vector3 targetPos = weaponOriginalPos + new Vector3(horizontalOffset, 0, 0);
        Quaternion targetRot = weaponOriginalRot * Quaternion.Euler(0, 0, tiltAngle);

        weaponModel.localPosition = Vector3.Lerp(weaponModel.localPosition, targetPos, Time.deltaTime * bobbingLerpSpeed);

        // Solo aplicar rotación si no está en retroceso
        if (!isRecoiling)
            weaponModel.localRotation = Quaternion.Slerp(weaponModel.localRotation, targetRot, Time.deltaTime * bobbingLerpSpeed);
    }

    private void ResetWeaponPosition()
    {
        if (weaponModel == null) return;

        walkTimer = 0f;
        weaponModel.localPosition = Vector3.Lerp(
            weaponModel.localPosition,
            weaponOriginalPos,
            Time.deltaTime * bobbingLerpSpeed
        );

        // Solo resetear rotación si no está en retroceso
        if (!isRecoiling)
        {
            weaponModel.localRotation = Quaternion.Slerp(
                weaponModel.localRotation,
                weaponOriginalRot,
                Time.deltaTime * bobbingLerpSpeed
            );
        }
    }

    // ==================== RETROCESO AL DISPARAR ====================
    private IEnumerator PlayRecoilAnimation()
    {
        if (weaponModel == null || !enableRecoil) yield break;

        isRecoiling = true;

        // Rotación de retroceso (la punta baja)
        Quaternion recoilRot = weaponOriginalRot * Quaternion.Euler(recoilRotation, 0, 0);

        float elapsed = 0f;

        // FASE 1: Bajar el arma rápidamente
        while (elapsed < 1f)
        {
            weaponModel.localRotation = Quaternion.Slerp(
                weaponModel.localRotation,
                recoilRot,
                elapsed
            );
            elapsed += Time.deltaTime * recoilDownSpeed;
            yield return null;
        }

        // Asegurar que llegó a la posición de retroceso
        weaponModel.localRotation = recoilRot;

        elapsed = 0f;

        // FASE 2: Subir el arma durante todo el cooldown
        while (elapsed < fireCooldown)
        {
            float t = elapsed / fireCooldown;
            weaponModel.localRotation = Quaternion.Slerp(
                recoilRot,
                weaponOriginalRot,
                t
            );
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Asegurar que volvió a la posición original
        weaponModel.localRotation = weaponOriginalRot;
        isRecoiling = false;
    }

    // ==================== DISPARO ====================
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
            source.PlayOneShot(shootSound);
        }

        ActivateBubbles(harpoonObj);

        // Iniciar retroceso
        if (enableRecoil)
            StartCoroutine(PlayRecoilAnimation());

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

        ParticleSystem[] allBubbles = harpoonObj.GetComponentsInChildren<ParticleSystem>(true);

        Debug.Log($"Se encontraron {allBubbles.Length} ParticleSystems en el arpón.");

        if (allBubbles.Length > 0)
        {
            ParticleSystem bubbles = allBubbles[0];
            Debug.Log("Activando burbujas: " + bubbles.gameObject.name);

            bubbles.gameObject.SetActive(true);
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

    // ==================== UI Y OTROS ====================
    private void UpdateAmmoUI()
    {
        if (ammoText != null && playerInventory != null)
        {
            int current = playerInventory.CurrentHarpoons;
            int max = playerInventory.MaxHarpoons;
            ammoText.text = $"{current}/{max}";
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
        if (moveAction != null) moveAction.action.Enable();
    }

    private void OnDisable()
    {
        if (fireAction != null) fireAction.action.Disable();
        if (moveAction != null) moveAction.action.Disable();
    }
}