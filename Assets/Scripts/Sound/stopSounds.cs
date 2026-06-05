using UnityEngine;

public class stopSounds : MonoBehaviour
{
    public void StopAllMySources()
    {
        AudioSource[] audioSources = GetComponentsInChildren<AudioSource>();
        foreach (AudioSource source in audioSources)
        {
            source.Stop();
        }
    } 
}
