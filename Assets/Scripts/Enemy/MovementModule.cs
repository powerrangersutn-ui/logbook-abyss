// MovementModule.cs - Maneja el movimiento
using UnityEngine;

public class MovementModule : EnemyModule
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 3f;
    [SerializeField] private float acceleration = 10f;
    [SerializeField] private float deceleration = 15f;

    [Header("Water Physics")]
    [SerializeField] private float buoyancyForce = 2f;
    [SerializeField] private float waterDrag = 1f;
    [SerializeField] private bool applyBuoyancy = true;

    [Header("Movement Behavior")]
    [SerializeField] private bool smoothMovement = true;
    [SerializeField] private float maxVerticalAngle = 45f; // L�mite de inclinaci�n

    private Rigidbody rb;
    private Vector3 currentVelocity;

    protected override void OnInitialize()
    {
        rb = core.Rigidbody;
        rb.linearDamping = waterDrag;
    }

    public override void OnFixedUpdate()
    {
        if (!isActive) return;

        // Aplicar flotaci�n
        if (applyBuoyancy)
        {
            rb.AddForce(Vector3.up * buoyancyForce, ForceMode.Acceleration);
        }

        // Aplicar movimiento deseado
        if (sharedData.desiredVelocity != Vector3.zero)
        {
            MoveTowardsVelocity(sharedData.desiredVelocity);
        }
        else
        {
            // Decelerar si no hay movimiento deseado
            Decelerate();
        }

        // Rotar hacia la direcci�n deseada
        if (sharedData.desiredDirection != Vector3.zero)
        {
            RotateTowards(sharedData.desiredDirection);
        }

        sharedData.currentSpeed = rb.linearVelocity.magnitude;
    }

    private void MoveTowardsVelocity(Vector3 targetVelocity)
    {
        if (smoothMovement)
        {
            // Interpolaci�n suave
            currentVelocity = Vector3.Lerp(
                currentVelocity,
                targetVelocity * moveSpeed,
                acceleration * Time.fixedDeltaTime
            );
        }
        else
        {
            currentVelocity = targetVelocity * moveSpeed;
        }

        rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, currentVelocity, Time.fixedDeltaTime * 5f);
    }

    private void Decelerate()
    {
        rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, Vector3.zero, deceleration * Time.fixedDeltaTime);
    }

    private void RotateTowards(Vector3 direction)
    {
        if (direction == Vector3.zero) return;

        // Limitar �ngulo vertical para que no se volteen completamente
        Vector3 flatDirection = direction;
        flatDirection.y = Mathf.Clamp(direction.y, -Mathf.Tan(maxVerticalAngle * Mathf.Deg2Rad), Mathf.Tan(maxVerticalAngle * Mathf.Deg2Rad));

        Quaternion targetRotation = Quaternion.LookRotation(flatDirection);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.fixedDeltaTime
        );
    }

    // M�todos p�blicos para otros m�dulos
    public void SetDesiredVelocity(Vector3 velocity)
    {
        sharedData.desiredVelocity = velocity;
    }

    public void SetDesiredDirection(Vector3 direction)
    {
        sharedData.desiredDirection = direction;
    }

    public void MoveToPosition(Vector3 targetPosition, float speedMultiplier = 1f)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        sharedData.desiredVelocity = direction * speedMultiplier;
        sharedData.desiredDirection = direction;
    }

    public void Stop()
    {
        sharedData.desiredVelocity = Vector3.zero;
    }
}