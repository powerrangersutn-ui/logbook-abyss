using UnityEngine;
using UnityEngine.Audio;

public class AudioContoller : MonoBehaviour
{
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private Transform player, enemy;

    void Update()
    {
        float d = Vector3.Distance(player.position, enemy.position);
        float t = Mathf.InverseLerp(25f, 5f, d);
        float cutoff = Mathf.Lerp(22000f, 800f, t);
        mixer.SetFloat("MusicCutoff", cutoff);
    }
}
