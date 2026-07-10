using UnityEngine;

[CreateAssetMenu(menuName = "AI/Fish School Settings", fileName = "NewFishSchoolSettings")]
public class FishSchoolSettings : ScriptableObject
{
    [Header("Neighbor Detection")]
    [Tooltip("How far (in world units) a fish looks for neighbors to react to.")]
    [SerializeField] private float neighborDetectionRadius = 5f;

    [Header("Separation")]
    [Tooltip("Minimum distance kept from other fish, per axis (X, Y, Z). " +
             "Lets you shape the school flatter/taller/wider instead of a perfect sphere.")]
    [SerializeField] private Vector3 separationDistance = new Vector3(1.2f, 0.8f, 1.2f);
    [SerializeField] private float separationWeight = 1.6f;

    [Header("Alignment")]
    [SerializeField] private float alignmentWeight = 1.0f;

    [Header("Cohesion")]
    [SerializeField] private float cohesionWeight = 0.8f;

    [Header("Swim Bounds")]
    [Tooltip("How strongly fish get pushed back once they leave the swim volume.")]
    [SerializeField] private float boundsWeight = 3f;

    [Header("Obstacle / Player Avoidance")]
    [SerializeField] private LayerMask avoidanceLayers;
    [SerializeField] private float avoidanceDetectionRadius = 2.5f;
    [SerializeField] private float avoidanceWeight = 4f;

    [Header("Movement")]
    [SerializeField] private float minSpeed = 0.8f;
    [SerializeField] private float maxSpeed = 2.5f;
    [SerializeField] private float maxSteerForce = 3f;
    [SerializeField] private float rotationSpeed = 6f;

    [Header("Panic / Scare")]
    [Tooltip("What triggers a panic reaction, e.g. the Player layer. Keep this " +
             "separate from Avoidance Layers so rocks cause gentle avoidance but the " +
             "player causes a scare.")]
    [SerializeField] private LayerMask scareLayers;
    [Tooltip("Distance at which fish notice a scare trigger and start fleeing.")]
    [SerializeField] private float scareDetectionRadius = 3.5f;
    [Tooltip("How long (seconds) a fish stays panicked after last sensing the trigger.")]
    [SerializeField] private float scareDuration = 1.5f;
    [Tooltip("Multiplies max speed while panicked.")]
    [SerializeField] private float scareSpeedMultiplier = 2.2f;
    [Tooltip("Multiplies the avoidance (flee) force while panicked.")]
    [SerializeField] private float scareAvoidanceMultiplier = 2.5f;
    [Tooltip("Multiplies turn speed while panicked, for sharper darting.")]
    [SerializeField] private float scareRotationMultiplier = 1.8f;
    [Tooltip("0 = fish ignore cohesion entirely while panicked (scatter). " +
             "1 = cohesion stays unaffected (school stays together while fleeing).")]
    [Range(0f, 1f)]
    [SerializeField] private float scareCohesionFactor = 0.15f;
    [Tooltip("Small random wobble added to the flee direction so fish don't scatter in perfect unison.")]
    [SerializeField] private float scareJitter = 1.5f;

    public float NeighborDetectionRadius => neighborDetectionRadius;
    public Vector3 SeparationDistance => separationDistance;
    public float SeparationWeight => separationWeight;
    public float AlignmentWeight => alignmentWeight;
    public float CohesionWeight => cohesionWeight;
    public float BoundsWeight => boundsWeight;
    public LayerMask AvoidanceLayers => avoidanceLayers;
    public float AvoidanceDetectionRadius => avoidanceDetectionRadius;
    public float AvoidanceWeight => avoidanceWeight;
    public float MinSpeed => minSpeed;
    public float MaxSpeed => maxSpeed;
    public float MaxSteerForce => maxSteerForce;
    public float RotationSpeed => rotationSpeed;

    public LayerMask ScareLayers => scareLayers;
    public float ScareDetectionRadius => scareDetectionRadius;
    public float ScareDuration => scareDuration;
    public float ScareSpeedMultiplier => scareSpeedMultiplier;
    public float ScareAvoidanceMultiplier => scareAvoidanceMultiplier;
    public float ScareRotationMultiplier => scareRotationMultiplier;
    public float ScareCohesionFactor => scareCohesionFactor;
    public float ScareJitter => scareJitter;

    private void OnValidate()
    {
        neighborDetectionRadius = Mathf.Max(0.1f, neighborDetectionRadius);

        separationDistance = new Vector3(
            Mathf.Max(0.05f, separationDistance.x),
            Mathf.Max(0.05f, separationDistance.y),
            Mathf.Max(0.05f, separationDistance.z));

        avoidanceDetectionRadius = Mathf.Max(0.1f, avoidanceDetectionRadius);

        minSpeed = Mathf.Max(0f, minSpeed);
        maxSpeed = Mathf.Max(minSpeed, maxSpeed);
        maxSteerForce = Mathf.Max(0.01f, maxSteerForce);
        rotationSpeed = Mathf.Max(0.01f, rotationSpeed);

        scareDetectionRadius = Mathf.Max(0.1f, scareDetectionRadius);
        scareDuration = Mathf.Max(0f, scareDuration);
        scareSpeedMultiplier = Mathf.Max(1f, scareSpeedMultiplier);
        scareAvoidanceMultiplier = Mathf.Max(1f, scareAvoidanceMultiplier);
        scareRotationMultiplier = Mathf.Max(1f, scareRotationMultiplier);
        scareJitter = Mathf.Max(0f, scareJitter);
    }
}