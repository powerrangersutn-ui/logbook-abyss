using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class FishSchoolManager : MonoBehaviour
{
    [Header("Spawning")]
    [SerializeField] private FishAgent fishPrefab;
    [SerializeField, Min(0)] private int fishCount = 20;
    [SerializeField] private bool spawnOnStart = true;

    [Header("Shared Behaviour")]
    [SerializeField] private FishSchoolSettings settings;

    [Header("Debug")]
    [SerializeField] private bool drawGizmos = true;

    private Collider boundsCollider;
    private readonly List<FishAgent> activeFish = new List<FishAgent>();

    public Collider BoundsVolume => boundsCollider;
    public FishSchoolSettings Settings => settings;
    public IReadOnlyList<FishAgent> ActiveFish => activeFish;

    private void Awake()
    {
        boundsCollider = GetComponent<Collider>();
        boundsCollider.isTrigger = true;

        if (settings == null)
        {
            Debug.LogError($"[{nameof(FishSchoolManager)}] No {nameof(FishSchoolSettings)} assigned on '{name}'.", this);
        }

        if (fishPrefab == null)
        {
            Debug.LogError($"[{nameof(FishSchoolManager)}] No fish prefab assigned on '{name}'.", this);
        }
    }

    private void Start()
    {
        if (spawnOnStart)
        {
            SpawnSchool();
        }
    }

    public void SpawnSchool()
    {
        if (fishPrefab == null || settings == null || boundsCollider == null)
        {
            return;
        }

        ClearSchool();

        for (int i = 0; i < fishCount; i++)
        {
            Vector3 spawnPosition = GetRandomPointInBounds();
            FishAgent fish = Instantiate(fishPrefab, spawnPosition, Random.rotation, transform);
            fish.Initialize(this, settings);
            activeFish.Add(fish);
        }
    }

    public void ClearSchool()
    {
        for (int i = activeFish.Count - 1; i >= 0; i--)
        {
            if (activeFish[i] != null)
            {
                Destroy(activeFish[i].gameObject);
            }
        }

        activeFish.Clear();
    }

    public void Unregister(FishAgent fish)
    {
        activeFish.Remove(fish);
    }

    private Vector3 GetRandomPointInBounds()
    {
        Bounds b = boundsCollider.bounds;
        Vector3 randomPoint;
        int safetyCounter = 0;

        do
        {
            randomPoint = new Vector3(
                Random.Range(b.min.x, b.max.x),
                Random.Range(b.min.y, b.max.y),
                Random.Range(b.min.z, b.max.z));
            safetyCounter++;
        }
        while (!IsInsideBounds(randomPoint) && safetyCounter < 30);

        return randomPoint;
    }

    private bool IsInsideBounds(Vector3 point)
    {
        return (boundsCollider.ClosestPoint(point) - point).sqrMagnitude <= 0.0001f;
    }

    private void OnDrawGizmos()
    {
        if (!drawGizmos)
        {
            return;
        }

        Collider col = boundsCollider != null ? boundsCollider : GetComponent<Collider>();
        if (col == null)
        {
            return;
        }

        Gizmos.color = new Color(0.2f, 0.8f, 1f, 0.35f);
        Bounds b = col.bounds;
        Gizmos.DrawWireCube(b.center, b.size);
    }
}