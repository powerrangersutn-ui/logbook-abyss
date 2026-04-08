using UnityEngine;

public class BulletScript : MonoBehaviour
{
    public int damage;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Untagged"))
        {
            Debug.Log(collision.gameObject.name);
            Destroy(gameObject);
        }
    }
}
