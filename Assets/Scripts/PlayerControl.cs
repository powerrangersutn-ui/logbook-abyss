using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    
    private Rigidbody rb;
    [Header("Movement")]
    [SerializeField] private float mvSpeed;

    [Header("Camera")]
    [SerializeField] private Vector2 sensitivity;
    [SerializeField] private Transform cameraTransform;
    private float camUpMax= 80;
    private float camDownMin = 280;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateMovement();
        UpdateMouseLook();
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

    //Seguir movimiento del mouse
    private void UpdateMouseLook()
    {
        float hor = Input.GetAxis("Mouse X");
        float ver = Input.GetAxis("Mouse Y");
        if (hor != 0)
        {
            transform.Rotate( 0, hor * sensitivity.x, 0);
        }

        if (ver != 0)
        {
            Vector3 rotation = cameraTransform.localEulerAngles;
            rotation.x = (rotation.x - ver * sensitivity.y + 360) % 360;
            //Limitar la cámara en su eje vertical
            if(rotation.x>camUpMax && rotation.x < 180)
            {
                rotation.x = camUpMax;
            }else if(rotation.x < camDownMin && rotation.x > 180)
            {
                rotation.x = camDownMin;
            }
            cameraTransform.localEulerAngles = rotation;
        }

    }
}
