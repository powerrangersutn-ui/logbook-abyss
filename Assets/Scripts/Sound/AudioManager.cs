using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private bool isMuted = false;
    public void ToggleMuteWithPause()
    {
        // true silences all audio, false resumes it
        AudioListener.pause = !AudioListener.pause;
    }
}
