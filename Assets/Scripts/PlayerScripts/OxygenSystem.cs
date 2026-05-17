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
        if (characterController != null)
        {
            float currentSpeed = characterController.velocity.magnitude;
            return currentSpeed > 7f; // Umbral entre caminar y correr
        }
        return false;
    }

    public void OnJump()
    {
        if (currentOxygen > 0)
        {
            ReduceOxygen(jumpOxygenCost);
        }
    }

    private void ReduceOxygen(float amount)
    {
        currentOxygen -= amount;
        currentOxygen = Mathf.Max(0, currentOxygen);

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

        OnOxygenChanged?.Invoke(OxygenPercentage);
    }

    public void RefillOxygen()
    {
        currentOxygen = maxOxygen;
        OnOxygenChanged?.Invoke(OxygenPercentage);
    }

    private void OxygenDepleted()
    {
        if (isDead) return;

        isDead = true;

        OnOxygenDepleted?.Invoke();

        // Notificar muerte al PlayerHealth
        PlayerHealth playerHealth = GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(999);
        }
    }
}