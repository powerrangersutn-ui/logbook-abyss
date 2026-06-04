using UnityEngine;
using UnityEngine.Audio;

public class ChangeMúsic : MonoBehaviour
{
    [SerializeField] private GameObject normalAmbience;
    [SerializeField] private GameObject chaseAmbience;

    [Header("AudioMixer Snapshots")]
    [SerializeField] private AudioMixerSnapshot snapshotChase;
    [SerializeField] private float transitionTime = 0f;

    private void Awake()
    {
        normalAmbience.SetActive(false);
        chaseAmbience.SetActive(true);

        snapshotChase.TransitionTo(transitionTime);
    }
}
