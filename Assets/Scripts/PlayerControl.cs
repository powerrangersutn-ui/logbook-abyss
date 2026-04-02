using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    private Rigidbody rb;

    [SerializeField] private float mvSpeed;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        float hor = Input.GetAxisRaw("Horizontal");
        float ver = Input.GetAxisRaw("Vertical");
        if (hor != 0 || ver != 0)
        {
            Debug.Log("se mueve");
            Vector3 direction = (transform.forward * ver + transform.right * hor).normalized;
            rb.linearVelocity = direction*mvSpeed;
        }
        else
        {
            rb.linearVelocity= Vector3.zero;
        }
    }

    
}
