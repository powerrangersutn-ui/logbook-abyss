using UnityEngine;

public class DialogTrigger : MonoBehaviour
{
    public string dialogID;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            DialogManager.Instance.ShowDialog(dialogID);
            Destroy(gameObject);
        }
    }
}
