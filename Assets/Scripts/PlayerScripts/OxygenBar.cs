using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Componente simple para controlar la barra de oxígeno en la UI.
/// Tu equipo de UI solo necesita arrastrarlo a un GameObject con una Image (barra de fill).
/// </summary>
public class OxygenBar : MonoBehaviour
{
    [Header("Referencias UI")]
    [Tooltip("La imagen que se llenará (Image con Type: Filled)")]
    [SerializeField] private Image fillImage;

    [Tooltip("Texto opcional que muestra el porcentaje (ej: '75%')")]
    [SerializeField] private TextMeshProUGUI percentageText;

    [Header("Configuración Visual")]
    [Tooltip("Color cuando el oxígeno está alto (>50%)")]
    [SerializeField] private Color highOxygenColor = new Color(0.2f, 0.8f, 1f); // Celeste

    [Tooltip("Color cuando el oxígeno está medio (15-50%)")]
    [SerializeField] private Color mediumOxygenColor = Color.yellow;

    [Tooltip("Color cuando el oxígeno está bajo (<=15%)")]
    [SerializeField] private Color lowOxygenColor = Color.red;

    [Header("Animación (Opcional)")]
    [SerializeField] private bool animateTransition = true;
    [SerializeField] private float transitionSpeed = 5f;

    [Header("Efecto de Parpadeo en Oxígeno Bajo")]
    [SerializeField] private bool blinkWhenLow = true;
    [SerializeField] private float blinkSpeed = 2f;

    private float targetFillAmount = 1f;
    private float currentFillAmount = 1f;

    private void Awake()
    {
        // Auto-asignar si no está configurado
        if (fillImage == null)
        {
            fillImage = GetComponent<Image>();
        }

        // Configurar valores iniciales
        if (fillImage != null)
        {
            fillImage.fillAmount = 1f;
            fillImage.color = highOxygenColor;
        }
    }

    /// <summary>
    /// Actualiza la barra de oxígeno con el porcentaje actual (0-100)
    /// </summary>
    public void UpdateOxygenBar(float oxygenPercentage)
    {
        if (fillImage == null) return;

        // Convertir porcentaje a fill amount (0-1)
        targetFillAmount = Mathf.Clamp01(oxygenPercentage / 100f);

        // Actualizar inmediatamente si no hay animación
        if (!animateTransition)
        {
            currentFillAmount = targetFillAmount;
            fillImage.fillAmount = currentFillAmount;
        }

        // Actualizar color según el nivel
        UpdateBarColor(oxygenPercentage);

        // Actualizar texto si existe
        if (percentageText != null)
        {
            percentageText.text = $"{Mathf.RoundToInt(oxygenPercentage)}%";
        }
    }

    private void Update()
    {
        // Animar la transición de la barra
        if (animateTransition && fillImage != null)
        {
            currentFillAmount = Mathf.Lerp(currentFillAmount, targetFillAmount, Time.deltaTime * transitionSpeed);
            fillImage.fillAmount = currentFillAmount;
        }

        // Parpadeo cuando está bajo
        if (blinkWhenLow && fillImage != null && targetFillAmount <= 0.15f)
        {
            float alpha = (Mathf.Sin(Time.time * blinkSpeed * Mathf.PI) + 1f) / 2f;
            alpha = Mathf.Lerp(0.5f, 1f, alpha); // Rango de 0.5 a 1

            Color currentColor = fillImage.color;
            currentColor.a = alpha;
            fillImage.color = currentColor;
        }
    }

    private void UpdateBarColor(float percentage)
    {
        if (fillImage == null) return;

        Color targetColor;

        if (percentage <= 15f)
        {
            targetColor = lowOxygenColor;
        }
        else if (percentage <= 50f)
        {
            // Interpolar entre bajo y medio
            float t = (percentage - 15f) / 35f; // Normalizar 15-50 a 0-1
            targetColor = Color.Lerp(lowOxygenColor, mediumOxygenColor, t);
        }
        else
        {
            // Interpolar entre medio y alto
            float t = (percentage - 50f) / 50f; // Normalizar 50-100 a 0-1
            targetColor = Color.Lerp(mediumOxygenColor, highOxygenColor, t);
        }

        // Mantener el alpha actual si está parpadeando
        if (blinkWhenLow && percentage <= 15f)
        {
            targetColor.a = fillImage.color.a;
        }

        fillImage.color = targetColor;
    }

    /// <summary>
    /// Resetea la barra al máximo (útil al reiniciar nivel)
    /// </summary>
    public void ResetBar()
    {
        targetFillAmount = 1f;
        currentFillAmount = 1f;

        if (fillImage != null)
        {
            fillImage.fillAmount = 1f;
            fillImage.color = highOxygenColor;
        }

        if (percentageText != null)
        {
            percentageText.text = "100%";
        }
    }
}