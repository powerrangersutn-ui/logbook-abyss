using System.Collections.Generic;
using UnityEngine;

public class DialogTrigger : MonoBehaviour
{
    public string dialogID;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip audioClip;

    [SerializeField] private stopSounds stopSounds;

   // private Dictionary<string, int> dialogData= new Dictionary<string, int>() { {"hint_intro", 0}, {"hint_hole", 1}, {"hint_fightEnemy",2}, { "hint_goal" , 3 }, { "hint_logbook", 4}, { "hint_elevator", 5} };

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //audioSource.PlayOneShot(audioClips[dialogData[dialogID]]);
            if (audioClip != null)
            {
                stopSounds.StopAllMySources();
                audioSource.PlayOneShot(audioClip);
            }

            DialogManager.Instance.ShowDialog(dialogID);
            Destroy(gameObject,5);
        }
    }
}
