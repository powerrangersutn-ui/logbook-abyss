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
        UpdateMovement();
    }

    //movimiento wasd
    private void UpdateMovement()
    {
        float hor = Input.GetAxisRaw("Horizontal");
        float ver = Input.GetAxisRaw("Vertical");
        Vector3 mVelocity = Vector3.zero;
        if (hor != 0 || ver != 0)
        {
            Debug.Log("se mueve");
            Vector3 direction = (transform.forward * ver + transform.right * hor).normalized;
            mVelocity = direction * mvSpeed;
        }
        mVelocity.y = rb.linearVelocity.y;
        rb.linearVelocity = mVelocity;
    }
}
