using UnityEngine;
using System.Collections;

public class CollapsingBridge : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private FallingPillar pillar;
    [SerializeField] private Collider triggerCollider;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip warningSound;
    [SerializeField] private AudioClip crackSound;
    [SerializeField][Range(0f, 1f)] private float warningSoundVolume = 0.7f;
    [SerializeField][Range(0f, 1f)] private float crackSoundVolume = 0.8f;

    [Header("Configuración")]
    [SerializeField] private float collapseDelay = 1.5f;

    private bool isTriggered = false;

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

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") || isTriggered) return;

        Debug.Log($"¡Jugador pisó el puente {gameObject.name}!");

        isTriggered = true;

        if (warningSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(warningSound, warningSoundVolume);
        }

        StartCoroutine(CollapseSequence());
    }

    private IEnumerator CollapseSequence()
    {
        yield return new WaitForSeconds(collapseDelay);

        if (crackSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(crackSound, crackSoundVolume);
        }

        if (pillar != null)
        {
            pillar.Collapse();
        }

        Debug.Log($"¡El puente {gameObject.name} ha colapsado!");
    }

    public void ForceCollapse()
    {
        if (!isTriggered && pillar != null)
        {
            isTriggered = true;
            StopAllCoroutines();
            pillar.Collapse();
        }
    }
}