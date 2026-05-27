using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public void ToggleMuteWithPause()
    {
        // true silences all audio, false resumes it
        AudioListener.pause = !AudioListener.pause;
    }
}
