using UnityEngine;

public class MenuSounds : MonoBehaviour
{
    private AudioSource m_audiosource;
    [SerializeField] private AudioClip switchSound;
    [SerializeField] private AudioClip clickSound;
    private void Start()
    {
        m_audiosource = GetComponent<AudioSource>();
    }

    public void ClickAudioOn()
    {
        m_audiosource.PlayOneShot(clickSound);
    }

    public void SwitchAudioOn()
    {
        m_audiosource.PlayOneShot(switchSound);
    }
}
