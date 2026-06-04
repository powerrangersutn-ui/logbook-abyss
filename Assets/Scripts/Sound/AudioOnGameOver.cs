using UnityEngine;
using UnityEngine.Audio;

public class AudioOnGameOver : MonoBehaviour
{
    [Header("AudioMixer Snapshots")]
    [SerializeField] private AudioMixerSnapshot snapshotDefault;
    [SerializeField] private AudioMixerSnapshot snapshotPause;
    [SerializeField] private float transitionTime = 0f;

    private void Awake()
    {
        snapshotPause.TransitionTo(transitionTime);
    }

    public void audioBackToNormal()
    {
        snapshotDefault.TransitionTo(transitionTime);
    }
}
