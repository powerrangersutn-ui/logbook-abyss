using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class AquaticLocomotion : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 6f;

    [SerializeField] private float acceleration = 8f;

    [SerializeField] private float rotationSpeed = 5f;

    [Header("References")]
    [SerializeField] private ObstacleAvoidance avoidance;

    private Rigidbody rb;
    private Vector3 desiredDirection;
    private float currentMoveSpeed;
    private bool overrideRotation;
    private Vector3 forcedLookDirection;
    private bool movementLocked;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        rb.useGravity = false;
        currentMoveSpeed = moveSpeed;
    }
    public void SetForcedLookDirection(Vector3 direction)
    {
        overrideRotation = true;

        forcedLookDirection = direction.normalized;
    }

    public void ClearForcedLookDirection()
    {
        overrideRotation = false;
    }

    public void SetMoveSpeed(float speed)
    {
        currentMoveSpeed = speed;
    }

    public void SetDesiredDirection(Vector3 direction)
    {
        desiredDirection = direction.normalized;
    }

    private void FixedUpdate()
    {
        Move();
        Rotate();
    }
    public void SetMovementLocked(bool locked)
    {
        movementLocked = locked;
    }
    private void Move()
    {
        if (movementLocked)
            return;
        Vector3 avoidanceDirection = Vector3.zero;

        if (avoidance != null)
            avoidanceDirection = avoidance.GetAvoidanceDirection();

        Vector3 finalDirection =
            (desiredDirection + avoidanceDirection).normalized;

        Vector3 desiredVelocity =
            finalDirection * currentMoveSpeed;

        Vector3 velocityChange =
            desiredVelocity - rb.linearVelocity;

        rb.AddForce(
            velocityChange * acceleration,
            ForceMode.Acceleration);
    }

    private void Rotate()
    {
        Vector3 rotationDirection;

        if (overrideRotation)
        {
            rotationDirection = forcedLookDirection;
        }
        else
        {
            rotationDirection = rb.linearVelocity;
        }

        if (rotationDirection.sqrMagnitude < 0.1f)
            return;

        Quaternion targetRotation =
            Quaternion.LookRotation(rotationDirection.normalized);

        Quaternion rotation =
            Quaternion.Slerp(
                rb.rotation,
                targetRotation,
                rotationSpeed * Time.fixedDeltaTime);

        rb.MoveRotation(rotation);
    }
}