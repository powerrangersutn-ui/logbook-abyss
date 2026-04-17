using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Runtime.CompilerServices;

public class HealthBarController : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Image healthBarFill;
    [SerializeField] private TextMeshProUGUI percentageText;

    [Header("Settings")]
    private Color fullHealthColor = Color.green;
    private Color lowHealthColor = Color.red;


    public void UpdateHealthBar(float healthPercentage)
    {
        //La barra necesita un valor entre 0 a 1 (siendo 0.5 el 50%)
        float clampedPercentage = Mathf.Clamp01(healthPercentage);

        // El relleno de la imagen (Requiere Image Type: Filled)
        healthBarFill.fillAmount = clampedPercentage;

        percentageText.text = (clampedPercentage * 100f).ToString("F0") + "%";

        //Esta opción sería para que el color sea gradual (verde, amarillo, naranja, rojo)
        //healthBarFill.color = Color.Lerp(lowHealthColor, fullHealthColor, clampedPercentage);

        if (clampedPercentage <= 0.2f)
        {
            healthBarFill.color = lowHealthColor;
        }
        else
        {
            healthBarFill.color = fullHealthColor;
        }
    }
}