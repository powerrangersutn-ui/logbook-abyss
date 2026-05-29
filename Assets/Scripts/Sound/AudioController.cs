using UnityEngine;
using UnityEngine.Audio;

public class AudioController : MonoBehaviour
{
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private Transform player;
    [SerializeField] private Transform[] enemies;

    void Update()
    {
        float minDistance = Mathf.Infinity;

        foreach (Transform enemy in enemies)
        {
            float d = Vector3.Distance(player.position, enemy.position);

            if (d < minDistance)
                minDistance = d;
        }

        float t = Mathf.InverseLerp(25f, 5f, minDistance);
        float cutoff = Mathf.Lerp(22000f, 800f, t);

        mixer.SetFloat("MusicCutoff", cutoff);
    }
}
