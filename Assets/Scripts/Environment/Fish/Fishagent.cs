using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FishAgent : MonoBehaviour
{
    private Rigidbody rb;
    private FishSchoolManager school;
    private FishSchoolSettings settings;

    private Vector3 currentVelocity;
    private float startleTimer;

    private readonly Collider[] avoidanceHitsBuffer = new Collider[16];
    private readonly Collider[] scareHitsBuffer = new Collider[8];

    public Vector3 CurrentVelocity => currentVelocity;
    public bool IsStartled => startleTimer > 0f;

    public void Initialize(FishSchoolManager schoolManager, FishSchoolSettings fishSettings)
    {
        school = schoolManager;
        settings = fishSettings;

        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.linearDamping = 0f;
        rb.angularDamping = 0f;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        currentVelocity = Random.insideUnitSphere * Mathf.Max(settings.MinSpeed, 0.1f);
    }

    private void FixedUpdate()
    {
        if (school == null || settings == null)
        {
            return;
        }

        UpdateStartleState();

        float avoidanceMultiplier = IsStartled ? settings.ScareAvoidanceMultiplier : 1f;

        Vector3 steering = ComputeFlockingSteering();
        steering += ComputeBoundsSteering() * settings.BoundsWeight;
        steering += ComputeAvoidanceSteering() * settings.AvoidanceWeight * avoidanceMultiplier;
        steering = Vector3.ClampMagnitude(steering, settings.MaxSteerForce * avoidanceMultiplier);

        currentVelocity += steering * Time.fixedDeltaTime;
        currentVelocity = ClampSpeed(currentVelocity);

        rb.MovePosition(rb.position + currentVelocity * Time.fixedDeltaTime);

        if (currentVelocity.sqrMagnitude > 0.0001f)
        {
            float rotationSpeed = settings.RotationSpeed * (IsStartled ? settings.ScareRotationMultiplier : 1f);
            Quaternion targetRotation = Quaternion.LookRotation(currentVelocity.normalized);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime));
        }
    }

    private void UpdateStartleState()
    {
        int hitCount = Physics.OverlapSphereNonAlloc(
            transform.position,
            settings.ScareDetectionRadius,
            scareHitsBuffer,
            settings.ScareLayers);

        if (hitCount > 0)
        {
            startleTimer = settings.ScareDuration;
        }
        else if (startleTimer > 0f)
        {
            startleTimer -= Time.fixedDeltaTime;
        }
    }

    private Vector3 ComputeFlockingSteering()
    {
        Vector3 separation = Vector3.zero;
        Vector3 alignment = Vector3.zero;
        Vector3 cohesion = Vector3.zero;
        int neighborCount = 0;

        var neighbors = school.ActiveFish;
        for (int i = 0; i < neighbors.Count; i++)
        {
            FishAgent other = neighbors[i];
            if (other == null || other == this)
            {
                continue;
            }

            Vector3 offset = transform.position - other.transform.position;
            float distance = offset.magnitude;

            if (distance > settings.NeighborDetectionRadius || distance <= 0.0001f)
            {
                continue;
            }

            neighborCount++;
            alignment += other.CurrentVelocity;
            cohesion += other.transform.position;

            Vector3 scaledOffset = new Vector3(
                offset.x / settings.SeparationDistance.x,
                offset.y / settings.SeparationDistance.y,
                offset.z / settings.SeparationDistance.z);

            if (scaledOffset.magnitude < 1f)
            {
                separation += offset.normalized / Mathf.Max(distance, 0.01f);
            }
        }

        if (neighborCount == 0)
        {
            return Vector3.zero;
        }

        alignment = (alignment / neighborCount) - currentVelocity;
        cohesion = (cohesion / neighborCount) - transform.position;

        float cohesionWeight = settings.CohesionWeight * (IsStartled ? settings.ScareCohesionFactor : 1f);

        return (separation * settings.SeparationWeight)
             + (alignment * settings.AlignmentWeight)
             + (cohesion * cohesionWeight);
    }

    private Vector3 ComputeBoundsSteering()
    {
        Collider bounds = school.BoundsVolume;
        Vector3 closestPoint = bounds.ClosestPoint(transform.position);

        if ((closestPoint - transform.position).sqrMagnitude <= 0.0001f)
        {
            return Vector3.zero;
        }

        return (closestPoint - transform.position).normalized;
    }

    private Vector3 ComputeAvoidanceSteering()
    {
        int hitCount = Physics.OverlapSphereNonAlloc(
            transform.position,
            settings.AvoidanceDetectionRadius,
            avoidanceHitsBuffer,
            settings.AvoidanceLayers);

        if (hitCount == 0)
        {
            return Vector3.zero;
        }

        Vector3 avoidance = Vector3.zero;
        for (int i = 0; i < hitCount; i++)
        {
            Collider hit = avoidanceHitsBuffer[i];
            if (hit == null)
            {
                continue;
            }

            Vector3 closest = hit.ClosestPoint(transform.position);
            Vector3 away = transform.position - closest;
            float distance = away.magnitude;

            if (distance <= 0.0001f)
            {
                continue;
            }

            avoidance += away.normalized / distance;
        }

        if (IsStartled && settings.ScareJitter > 0f)
        {
            avoidance += Random.insideUnitSphere * settings.ScareJitter;
        }

        return avoidance;
    }

    private Vector3 ClampSpeed(Vector3 velocity)
    {
        float speed = velocity.magnitude;
        if (speed < 0.0001f)
        {
            return velocity;
        }

        float clampedSpeed = Mathf.Clamp(speed, settings.MinSpeed, settings.MaxSpeed);
        return velocity.normalized * clampedSpeed;
    }

    private void OnDestroy()
    {
        if (school != null)
        {
            school.Unregister(this);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (settings == null)
        {
            return;
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, settings.AvoidanceDetectionRadius);

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, settings.ScareDetectionRadius);

        Gizmos.color = Color.cyan;
        Matrix4x4 previousMatrix = Gizmos.matrix;
        Gizmos.matrix = Matrix4x4.TRS(transform.position, Quaternion.identity, settings.SeparationDistance * 2f);
        Gizmos.DrawWireSphere(Vector3.zero, 0.5f);
        Gizmos.matrix = previousMatrix;
    }
}