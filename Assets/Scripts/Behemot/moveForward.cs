using UnityEngine;

public class moveForward : MonoBehaviour
{
    [SerializeField] private float speed = 15.0f;

    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }
}
