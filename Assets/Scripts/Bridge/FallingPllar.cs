using UnityEngine;

public class FallingPillar : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip collapseSound;
    [SerializeField][Range(0f, 1f)] private float collapseSoundVolume = 1f;

    [Header("Configuración")]
    [SerializeField] private bool destroyOnCollapse = false;
    [SerializeField] private float destroyDelay = 0f;

    private bool hasCollapsed = false;

    private void Awake()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 1f;
        }
    }

    public void Collapse()
    {
        if (hasCollapsed) return;

        hasCollapsed = true;

        Debug.Log($"¡Pilar {gameObject.name} colapsando!");

        if (collapseSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(collapseSound, collapseSoundVolume);
        }

        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = false;
        }

        MeshRenderer renderer = GetComponent<MeshRenderer>();
        if (renderer != null)
        {
            renderer.enabled = false;
        }

        if (destroyOnCollapse)
        {
            if (destroyDelay > 0)
                Destroy(gameObject, destroyDelay);
            else
                Destroy(gameObject);
        }
    }

    public void Reset()
    {
        hasCollapsed = false;

        Collider col = GetComponent<Collider>();
        if (col != null)
            col.enabled = true;

        MeshRenderer renderer = GetComponent<MeshRenderer>();
        if (renderer != null)
            renderer.enabled = true;
    }
}