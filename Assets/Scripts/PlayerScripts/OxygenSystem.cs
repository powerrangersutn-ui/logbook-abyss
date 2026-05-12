using UnityEngine;
using UnityEngine.Events;

public class OxygenSystem : MonoBehaviour
{
    [Header("Configuración de Oxígeno")]
    [SerializeField] private float maxOxygen = 100f;
    [SerializeField] private float currentOxygen = 100f;

    [Header("Reducción de Oxígeno")]
    [Tooltip("Oxígeno que se reduce por segundo estando quieto o caminando")]
    [SerializeField] private float oxygenDepletionRate = 1f;

    [Tooltip("Multiplicador de reducción al correr (ejemplo: 2 = consume el doble)")]
    [SerializeField] private float sprintDepletionMultiplier = 2f;

    [Tooltip("Cantidad de oxígeno que se reduce al saltar")]
    [SerializeField] private float jumpOxygenCost = 5f;

    [Header("Referencias UI")]
    [SerializeField] private OxygenBar oxygenBar;

    [Header("Eventos")]
    public UnityEvent OnOxygenDepleted;
    public UnityEvent<float> OnOxygenChanged; // Pasa el porcentaje actual

    // Referencias
    private PlayerControl playerControl;
    private CharacterController characterController;
    private bool isDead = false;

    // Propiedades públicas
    public float CurrentOxygen => currentOxygen;
    public float MaxOxygen => maxOxygen;
    public float OxygenPercentage => (currentOxygen / maxOxygen) * 100f;
    public bool IsLowOxygen => OxygenPercentage <= 15f;

    private void Awake()
    {
        playerControl = GetComponent<PlayerControl>();
        characterController = GetComponent<CharacterController>();
        currentOxygen = maxOxygen;
    }

    private void Start()
    {
        UpdateOxygenUI();
    }

    private void Update()
    {
        if (isDead || GameManager.Instance.gameEnded) return;

        ReduceOxygenOverTime();
    }

    private void ReduceOxygenOverTime()
    {
        if (currentOxygen <= 0) return;

        // Determinar si está corriendo
        bool isSprinting = IsSprinting();

        // Calcular reducción
        float depletionMultiplier = isSprinting ? sprintDepletionMultiplier : 1f;
        float oxygenToReduce = oxygenDepletionRate * depletionMultiplier * Time.deltaTime;

        // Reducir oxígeno
        ReduceOxygen(oxygenToReduce);
    }

    private bool IsSprinting()
    {
        // Verificar si el jugador está corriendo
        // Necesitamos acceder al Input Action de Sprint desde PlayerControl
        // Como no tenemos acceso directo, verificamos la velocidad
        if (characterController != null)
        {
            float currentSpeed = characterController.velocity.magnitude;
            // Si la velocidad es mayor que walkSpeed, está corriendo
            // Asumimos que walkSpeed normal es ~5, sprint es ~10
            return currentSpeed > 7f; // Umbral entre caminar y correr
        }
        return false;
    }

    public void OnJump()
    {
        // Llamar este método cuando el jugador salte
        if (currentOxygen > 0)
        {
            ReduceOxygen(jumpOxygenCost);
            Debug.Log($"[OxygenSystem] Saltó! Oxígeno reducido en {jumpOxygenCost}. Actual: {currentOxygen:F1}");
        }
    }

    private void ReduceOxygen(float amount)
    {
        currentOxygen -= amount;
        currentOxygen = Mathf.Max(0, currentOxygen);

        UpdateOxygenUI();
        OnOxygenChanged?.Invoke(OxygenPercentage);

        if (currentOxygen <= 0)
        {
            OxygenDepleted();
        }
    }

    public void AddOxygen(float amount)
    {
        if (isDead) return;

        float oxygenBefore = currentOxygen;
        currentOxygen += amount;
        currentOxygen = Mathf.Min(currentOxygen, maxOxygen);

        UpdateOxygenUI();
        OnOxygenChanged?.Invoke(OxygenPercentage);

        Debug.Log($"[OxygenSystem] Oxígeno añadido: {amount:F1} | Antes: {oxygenBefore:F1} → Después: {currentOxygen:F1}");
    }

    public void RefillOxygen()
    {
        currentOxygen = maxOxygen;
        UpdateOxygenUI();
        OnOxygenChanged?.Invoke(OxygenPercentage);
        Debug.Log("[OxygenSystem] Oxígeno restaurado al máximo");
    }

    private void OxygenDepleted()
    {
        if (isDead) return;

        isDead = true;
        Debug.Log("[OxygenSystem] ¡OXÍGENO AGOTADO! Jugador muere por asfixia");

        OnOxygenDepleted?.Invoke();

        // Notificar muerte al PlayerHealth
        PlayerHealth playerHealth = GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            // Hacer daño masivo para matar al jugador
            playerHealth.TakeDamage(999);
        }
    }

    private void UpdateOxygenUI()
    {
        if (oxygenBar != null)
        {
            oxygenBar.UpdateOxygenBar(OxygenPercentage);
        }
    }

    // Método para debugging en el inspector
    private void OnGUI()
    {
        if (Application.isEditor)
        {
            GUI.Label(new Rect(10, 100, 300, 20), $"Oxígeno: {currentOxygen:F1}/{maxOxygen} ({OxygenPercentage:F1}%)");
            GUI.Label(new Rect(10, 120, 300, 20), $"Estado: {(IsLowOxygen ? "BAJO" : "Normal")}");
        }
    }
}