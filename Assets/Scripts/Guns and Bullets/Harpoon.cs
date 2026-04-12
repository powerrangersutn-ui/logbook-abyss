using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Harpoon : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float gravityScale = 9f;
    [SerializeField] private float linearDamping = 1.3f;
    [SerializeField] private float angularDamping = 2f;

    [Header("Daño")]
    [SerializeField] private int damageToEnemy = 50;

    [Header("Colisiones")]
    [SerializeField] private LayerMask groundLayers;
    [SerializeField] private string[] breakTags;
    [SerializeField] private string enemyTag = "Enemy";

    [Header("Recolección")]
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private float minimumSpeedToStick = 4f;
    [SerializeField] private float pickupRadius = 1.2f;        // ← Tamaño del área de recolección

    private Rigidbody rb;
    private Collider mainCollider;          // Collider físico (no trigger)
    private SphereCollider pickupTrigger;   // Trigger grande para recoger
    private bool isEmbedded;

    private HarpoonPool pool;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        mainCollider = GetComponent<Collider>();   // El collider original

        rb.useGravity = false;
        rb.linearDamping = linearDamping;
        rb.angularDamping = angularDamping;

        CreatePickupTrigger();
    }

    private void CreatePickupTrigger()
    {
        // Creamos un SphereCollider como trigger grande
        pickupTrigger = gameObject.AddComponent<SphereCollider>();
        pickupTrigger.radius = pickupRadius;
        pickupTrigger.isTrigger = true;
    }

    public void Initialize(HarpoonPool harpoonPool)
    {
        pool = harpoonPool;
    }

    public void Launch(Vector3 direction, float speed)
    {
        isEmbedded = false;
        if (rb != null)
        {
            rb.linearVelocity = direction.normalized * speed;
            rb.AddForce(Physics.gravity * gravityScale, ForceMode.Acceleration);
        }
    }

    public void ResetHarpoon()
    {
        isEmbedded = false;
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = false;
            rb.useGravity = false;
        }
        if (mainCollider != null)
            mainCollider.isTrigger = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isEmbedded) return;

        string hitTag = collision.gameObject.tag;

        if (hitTag == enemyTag)
        {
            EnemyHealth enemyHealth = collision.gameObject.GetComponent<EnemyHealth>();
            if (enemyHealth != null) enemyHealth.TakeDamage(damageToEnemy);
            else Destroy(collision.gameObject);

            if (rb != null) rb.useGravity = true;
            return;
        }

        if (IsBreakTag(hitTag))
        {
            ReturnToPool();
            return;
        }

        if (((1 << collision.gameObject.layer) & groundLayers.value) != 0)
        {
            if (rb.linearVelocity.magnitude >= minimumSpeedToStick)
            {
                Embed(collision.contacts[0].point);
            }
            else
            {
                ReturnToPool();
            }
        }
        else
        {
            ReturnToPool();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Solo recolectar si ya está clavado
        if (isEmbedded && other.CompareTag(playerTag))
        {
            if (pool != null && pool.GetHarpoonGun() != null)
            {
                pool.GetHarpoonGun().CollectHarpoon(gameObject);
            }
        }
    }

    private void Embed(Vector3 embedPoint)
    {
        isEmbedded = true;
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }
        transform.position = embedPoint;

        if (mainCollider != null)
            mainCollider.isTrigger = true;   // El collider físico pasa a trigger
    }

    private bool IsBreakTag(string tagToCheck)
    {
        if (breakTags == null) return false;
        foreach (string t in breakTags)
            if (t == tagToCheck) return true;
        return false;
    }

    private void ReturnToPool()
    {
        if (pool != null)
            pool.ReturnToPool(gameObject);
        else
            gameObject.SetActive(false);
    }
    private void FixedUpdate()
    {
        if (!isEmbedded && rb != null)
        {
            if (rb.linearVelocity.y > -18f)
                rb.AddForce(Physics.gravity * gravityScale, ForceMode.Acceleration);

            if (rb.linearVelocity.magnitude > 8f)
            {
                if (Physics.Raycast(transform.position, rb.linearVelocity.normalized,
                    out RaycastHit hit, rb.linearVelocity.magnitude * Time.fixedDeltaTime + 0.4f, groundLayers))
                {
                    Embed(hit.point);
                }
            }
        }
    }
}