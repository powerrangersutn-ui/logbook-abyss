using UnityEngine;
using UnityEngine.UI;
using TMPro;

//Es la Vista. Solo muestra datos.
public class HealthBarController : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Image healthBarFill;
    [SerializeField] private TextMeshProUGUI percentageText;


    [Header("Settings")]
    private Color fullHealthColor = Color.green;
    private Color lowHealthColor = Color.red;

    [Header("Broken Glass Effects")]
    [SerializeField] private GameObject brokenGlass25; // Panel 25%
    [SerializeField] private GameObject brokenGlass10; // Panel 10%

    private void Awake()
    {
        if (brokenGlass25 != null) brokenGlass25.SetActive(false);
        if (brokenGlass10 != null) brokenGlass10.SetActive(false);
    }

    private void OnEnable(){
        UIGameEvents.onPlayerHealthChanged += UpdateHealthBar;
        UIGameEvents.onPlayerDeath += HandleDeathUI;
    }

    private void OnDestroy()
    {
        UIGameEvents.onPlayerHealthChanged -= UpdateHealthBar;
        UIGameEvents.onPlayerDeath -= HandleDeathUI;
    }

    private void UpdateHealthBar(int healthPercentage, int maxHealth)
    {
        //La barra necesita un valor entre 0 a 1 (siendo 0.5 el 50%)
        float clampedPercentage = (float) healthPercentage / maxHealth;

        // El relleno de la imagen (Requiere Image Type: Filled)
        healthBarFill.fillAmount = clampedPercentage;

        percentageText.text = (clampedPercentage * 100f).ToString("F0") + "%";

        //Esta opción sería para que el color sea gradual (verde, amarillo, naranja, rojo)
        //healthBarFill.color = Color.Lerp(lowHealthColor, fullHealthColor, clampedPercentage);

        if (brokenGlass25 != null)
            brokenGlass25.SetActive(clampedPercentage <= 0.25f);

        if (brokenGlass10 != null)
            brokenGlass10.SetActive(clampedPercentage <= 0.10f);

        if (clampedPercentage <= 0.2f)
        {
            healthBarFill.color = lowHealthColor;
        }
        else
        {
            healthBarFill.color = fullHealthColor;
        }
    }

    private void HandleDeathUI()
    {
        healthBarFill.fillAmount = 0f;
    }
}