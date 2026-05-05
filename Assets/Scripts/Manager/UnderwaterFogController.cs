using UnityEngine;

[ExecuteInEditMode]
public class UnderwaterFogController : MonoBehaviour
{
    [Header("Configuracion FOG")]
    [SerializeField]
    [Tooltip("Color FOG bajo el agua")]
    private Color fogColor = new Color(0.1f, 0.3f, 0.4f, 1f);

    [Header("Visibilidad")]
    [SerializeField]
    [Tooltip("Minimo distancia FOG")]
    [Range(0f, 100f)]
    private float startDistance = 5f;

    [SerializeField]
    [Tooltip("Maxima distancia FOG")]
    [Range(1f, 500f)]
    private float endDistance = 50f;

    [Header("Densidad")]
    [SerializeField]
    [Tooltip("FOG dencidad en exposicion")]
    [Range(0f, 0.1f)]
    private float density = 0.02f;

    [Header("Fog Mode")]
    [SerializeField]
    [Tooltip("Forma de calcularlo")]
    private FogMode fogMode = FogMode.ExponentialSquared;

    [Header("Opciones")]
    [SerializeField]
    [Tooltip("On/OFF fog")]
    private bool fogEnabled = true;

    // Propiedades publicas
    public Color FogColor
    {
        get => fogColor;
        set
        {
            fogColor = value;
            ApplyFog();
        }
    }

    public float StartDistance
    {
        get => startDistance;
        set
        {
            startDistance = Mathf.Max(0f, value);
            ApplyFog();
        }
    }

    public float EndDistance
    {
        get => endDistance;
        set
        {
            endDistance = Mathf.Max(startDistance + 1f, value);
            ApplyFog();
        }
    }

    public float Density
    {
        get => density;
        set
        {
            density = Mathf.Clamp(value, 0f, 0.1f);
            ApplyFog();
        }
    }

    public FogMode Mode
    {
        get => fogMode;
        set
        {
            fogMode = value;
            ApplyFog();
        }
    }

    public bool FogEnabled
    {
        get => fogEnabled;
        set
        {
            fogEnabled = value;
            ApplyFog();
        }
    }

    private void OnEnable()
    {
        ApplyFog();
    }

    private void Update()
    {
        ApplyFog();
    }

    private void OnDisable()
    {
        RenderSettings.fog = false;
    }

    private void ApplyFog()
    {
        RenderSettings.fog = fogEnabled;

        if (!fogEnabled) return;

        RenderSettings.fogColor = fogColor;
        RenderSettings.fogMode = fogMode;

        if (fogMode == FogMode.Linear)
        {
            RenderSettings.fogStartDistance = startDistance;
            RenderSettings.fogEndDistance = endDistance;
        }
        else
        {
            RenderSettings.fogDensity = density;
        }
    }

    public void SetDistances(float start, float end)
    {
        startDistance = Mathf.Max(0f, start);
        endDistance = Mathf.Max(start + 1f, end);
        ApplyFog();
    }

    public void SetUnderwaterFog(Color color, float newDensity)
    {
        fogColor = color;
        density = Mathf.Clamp(newDensity, 0f, 0.1f);
        ApplyFog();
    }

    public void ResetToDefault()
    {
        fogColor = new Color(0.1f, 0.3f, 0.4f, 1f);
        startDistance = 5f;
        endDistance = 50f;
        density = 0.02f;
        fogMode = FogMode.ExponentialSquared;
        fogEnabled = true;
        ApplyFog();
    }
}