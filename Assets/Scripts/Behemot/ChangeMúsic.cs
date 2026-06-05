using UnityEngine;
using UnityEngine.Audio;

public class ChangeMúsic : MonoBehaviour
{
    [SerializeField] private AudioSource musicAudio;
    [SerializeField] private AudioClip musicClip;

    [Header("AudioMixer Snapshots")]
    [SerializeField] private AudioMixerSnapshot snapshotChase;
    [SerializeField] private float transitionTime = 0f;

    private void Awake()
    {
        musicAudio.Stop();
        musicAudio.clip = musicClip;
        musicAudio.Play();
        snapshotChase.TransitionTo(transitionTime);
    }
}
