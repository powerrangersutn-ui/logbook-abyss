using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// Componente simple para controlar la barra de oxígeno en la UI.
public class OxygenBar : MonoBehaviour
{
    [Header("Referencias UI")]
    [Tooltip("Arrastrst las 10 imágenes que conforman la barra")]
    [SerializeField] private Image[] oxygenSegments;
    [SerializeField] private TextMeshProUGUI percentageText;

    [Header("Configuración Visual")]
    [Tooltip("El color fijo de la barra (Celeste).")]
    [SerializeField] private Color oxygenColor = new Color(0.2f, 0.8f, 1f); // Celeste

    [Header("Configuración de Opacidad")]
    [Tooltip("Opacidad cuando el segmento está lleno")]
    [Range(0f, 1f)][SerializeField] private float fullAlpha = 1f;

    [Tooltip("Opacidad cuando el segmento está vacío")]
    [Range(0f, 1f)][SerializeField] private float emptyAlpha = 0.2f;

    [Header("Animación (Opcional)")]
    [SerializeField] private bool animateTransition = true;
    [SerializeField] private float transitionSpeed = 5f;

    [Header("Efecto de Parpadeo en Oxígeno Bajo")]
    [SerializeField] private bool blinkWhenLow = true;
    [SerializeField] private float blinkSpeed = 2f;

    private float targetFillAmount = 1f;
    private float currentFillAmount = 1f;

    private void OnEnable()
    {
        UIGameEvents.onPlayerOxygenChanged += UpdateOxygenBarData;
    }

    private void OnDestroy()
    {
        UIGameEvents.onPlayerOxygenChanged -= UpdateOxygenBarData;
    }


    /// Actualiza la barra de oxígeno con el porcentaje actual (0-100)
    private void UpdateOxygenBarData(float oxygenPercentage)
    {
        targetFillAmount = Mathf.Clamp01(oxygenPercentage / 100f);

        if (!animateTransition)
        {
            currentFillAmount = targetFillAmount;
        }

        if (percentageText != null)
        {
            percentageText.text = $"{Mathf.RoundToInt(oxygenPercentage)}%";
        }
    }

    private void Update()
    {
        if (animateTransition)
        {
            currentFillAmount = Mathf.Lerp(currentFillAmount, targetFillAmount, Time.deltaTime * transitionSpeed);
        }
        else
        {
            currentFillAmount = targetFillAmount;
        }

        UpdateSegmentsVisuals();
    }

    /// Calcula y aplica la opacidad a cada segmento de la lista
    private void UpdateSegmentsVisuals()
    {
        if (oxygenSegments == null || oxygenSegments.Length == 0) return;

        float totalFill = currentFillAmount * oxygenSegments.Length;

        for (int i = 0; i < oxygenSegments.Length; i++)
        {
            if (oxygenSegments[i] == null) continue;

            float segmentFill = Mathf.Clamp01(totalFill - i);

            float currentAlpha = Mathf.Lerp(emptyAlpha, fullAlpha, segmentFill);

            if (blinkWhenLow && targetFillAmount <= 0.15f && segmentFill > 0)
            {
                float blinkFactor = (Mathf.Sin(Time.time * blinkSpeed * Mathf.PI) + 1f) / 2f;
                currentAlpha *= Mathf.Lerp(0.5f, 1f, blinkFactor);
            }

            Color appliedColor = oxygenColor;
            appliedColor.a = currentAlpha;
            oxygenSegments[i].color = appliedColor;
        }
    }

    public void ResetBar()
    {
        targetFillAmount = 1f;
        currentFillAmount = 1f;
        UpdateSegmentsVisuals();

        if (percentageText != null)
        {
            percentageText.text = "100%";
        }
    }
}