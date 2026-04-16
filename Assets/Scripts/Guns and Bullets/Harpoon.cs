using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Harpoon : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float gravityScale = 9f;
    [SerializeField] private float linearDamping = 1.3f;
    [SerializeField] private float angularDamping = 2f;

    [Header("Comportamiento al Impactar")]
    [SerializeField] private int damageToEnemy = 1;
    [SerializeField] private float embedDepth = 0.15f;

    [Header("Colisiones")]
    [SerializeField] private LayerMask groundLayers;
    [SerializeField] private string[] breakTags = { "Stone", "Metal" };
    [SerializeField] private string enemyTag = "Enemy";

    [Header("Recolección")]
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private float minimumSpeedToStick = 4f;
    [SerializeField] private float pickupRadius = 1.8f;
    [SerializeField] private bool canPickupFromGround = true;
    [SerializeField] private bool canPickupFromEnemy = true;

    [Header("Visual Feedback")]
    [SerializeField] private GameObject pickupIndicator;  // Opcional: sprite o partículas
    [SerializeField] private float indicatorPulseSpeed = 2f;

    private Rigidbody rb;
    private Collider mainCollider;
    private SphereCollider pickupTrigger;

    private bool isEmbedded;
    private bool hasDamaged;
    private bool isAvailableForPickup;
    private HarpoonPool pool;
    private Transform stuckParent;
    private EnemyHealth stuckEnemy;

    // Estados del arpón
    private enum HarpoonState
    {
        Flying,           // En el aire después de disparar
        EmbeddedInGround, // Clavado en el suelo
        EmbeddedInEnemy,  // Clavado en un enemigo
        Falling,          // Cayendo después de matar enemigo
        InPool            // Inactivo en el pool
    }
    private HarpoonState currentState = HarpoonState.InPool;

    #region Inicialización

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        mainCollider = GetComponent<Collider>();

        ConfigureRigidbody();
        CreatePickupTrigger();

        if (pickupIndicator != null)
            pickupIndicator.SetActive(false);
    }

    private void ConfigureRigidbody()
    {
        rb.useGravity = false;
        rb.linearDamping = linearDamping;
        rb.angularDamping = angularDamping;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
    }

    private void CreatePickupTrigger()
    {
        pickupTrigger = gameObject.AddComponent<SphereCollider>();
        pickupTrigger.radius = pickupRadius;
        pickupTrigger.isTrigger = true;
    }

    public void Initialize(HarpoonPool harpoonPool)
    {
        pool = harpoonPool;
    }

    #endregion

    #region Lanzamiento y Reset

    public void Launch(Vector3 direction, float speed)
    {
        ResetState();
        currentState = HarpoonState.Flying;
        rb.linearVelocity = direction.normalized * speed;
        AlignWithVelocity();
    }

    private void ResetState()
    {
        isEmbedded = false;
        hasDamaged = false;
        isAvailableForPickup = false;
        stuckParent = null;
        stuckEnemy = null;
        currentState = HarpoonState.InPool;

        rb.isKinematic = false;
        rb.useGravity = false;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        mainCollider.isTrigger = false;

        if (pickupIndicator != null)
            pickupIndicator.SetActive(false);

        CancelInvoke();
    }

    public void ResetForPool()
    {
        ResetState();
    }

    #endregion

    #region Colisiones

    private void OnCollisionEnter(Collision collision)
    {
        if (isEmbedded || currentState != HarpoonState.Flying) return;

        string hitTag = collision.gameObject.tag;

        // Se rompe contra piedra/metal
        if (IsBreakTag(hitTag))
        {
            BreakHarpoon();
            return;
        }

        // Golpea a un enemigo
        if (hitTag == enemyTag)
        {
            HandleEnemyHit(collision.gameObject, collision.contacts[0]);
            return;
        }

        // Golpea el terreno
        if (IsGroundLayer(collision.gameObject.layer))
        {
            HandleGroundHit(collision.contacts[0].point, collision.contacts[0].normal);
            return;
        }

        // Cualquier otra cosa → volver al pool
        ReturnToPool();
    }


    private void HandleEnemyHit(GameObject enemy, ContactPoint contact)
    {
        EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();

        if (enemyHealth != null && !hasDamaged)
        {
            int currentEnemyHealth = enemyHealth.GetCurrentHealth();
            bool willKillEnemy = (currentEnemyHealth <= damageToEnemy);

            // Hacer daño
            enemyHealth.TakeDamage(damageToEnemy);
            hasDamaged = true;

            if (willKillEnemy)
            {
                // ← El enemigo va a morir, NO nos pegamos
                Debug.Log($"Arpón mata al enemigo. Rebotando para ser recogible.");
                BounceOffEnemy(contact.normal);
            }
            else
            {
                // ← El enemigo sobrevive, nos pegamos normalmente
                StickToEnemy(enemy, contact.point, contact.normal);
            }
        }
        else
        {
            // Sin EnemyHealth o ya hicimos daño, solo rebotar
            BounceOffEnemy(contact.normal);
        }
    }

    private void BounceOffEnemy(Vector3 hitNormal)
    {
        currentState = HarpoonState.Falling;
        isEmbedded = false;

        // NO hacer SetParent - quedarse libre
        transform.SetParent(null);

        // Física de rebote/caída
        rb.isKinematic = false;
        rb.useGravity = true;
        rb.linearVelocity = hitNormal * 4f + Vector3.down * 2f;  // Rebote + caída
        rb.angularVelocity = Random.insideUnitSphere * 2f;       // Un poco de spin

        mainCollider.isTrigger = false;

        // Hacer recogible inmediatamente
        if (canPickupFromGround)
        {
            EnablePickup();
        }
    }

    private void HandleGroundHit(Vector3 hitPoint, Vector3 hitNormal)
    {
        if (rb.linearVelocity.magnitude >= minimumSpeedToStick)
        {
            EmbedInGround(hitPoint, hitNormal);
        }
        else
        {
            // Si no tiene suficiente velocidad, simplemente cae al suelo
            LandOnGround(hitPoint, hitNormal);
        }
    }

    #endregion

    #region Clavado en Enemigo

    private void StickToEnemy(GameObject enemy, Vector3 hitPoint, Vector3 hitNormal)
    {
        currentState = HarpoonState.EmbeddedInEnemy;
        isEmbedded = true;
        stuckParent = enemy.transform;
        stuckEnemy = enemy.GetComponent<EnemyHealth>();

        // Física
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;

        // Jerarquía
        transform.SetParent(enemy.transform, true);

        // Posicionamiento
        transform.position = hitPoint - transform.forward * embedDepth;
        transform.rotation = Quaternion.LookRotation(-hitNormal);

        // Colisión como trigger
        mainCollider.isTrigger = true;

        // Hacer recogible si está configurado
        if (canPickupFromEnemy)
        {
            EnablePickup();
        }

        // Suscribirse a la muerte del enemigo
        if (stuckEnemy != null)
        {
            stuckEnemy.OnHarpoonStuck(this);
        }
    }

    public void ReleaseFromEnemy()
    {
        if (stuckParent == null) return;

        currentState = HarpoonState.Falling;

        //guardar referencia antes de limpiar
        EnemyHealth tempEnemy = stuckEnemy;
        Transform tempParent = stuckParent;

        //limpiar referencias ANTES de cambiar el parent
        stuckEnemy = null;
        stuckParent = null;

        //separar del padre
        transform.SetParent(null);

        //fisica de caida
        rb.isKinematic = false;
        rb.useGravity = true;
        rb.linearVelocity = Vector3.down * 2f;

        isEmbedded = false;
        mainCollider.isTrigger = false;

        // hacer recogible inmediatamente
        if (canPickupFromGround)
        {
            EnablePickup();
        }

        //OPCIONAL: Notificar al enemigo (si todavía existe)
        if (tempEnemy != null)
        {
            tempEnemy.OnHarpoonRemoved(this);
        }
    }

    #endregion

    #region Clavado en Terreno

    private void EmbedInGround(Vector3 embedPoint, Vector3 embedNormal)
    {
        currentState = HarpoonState.EmbeddedInGround;
        isEmbedded = true;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;

        // Posicionar correctamente
        transform.position = embedPoint;
        transform.rotation = Quaternion.LookRotation(-embedNormal);

        mainCollider.isTrigger = true;

        // Hacer recogible
        if (canPickupFromGround)
        {
            EnablePickup();
        }
    }

    private void LandOnGround(Vector3 landPoint, Vector3 landNormal)
    {
        currentState = HarpoonState.EmbeddedInGround;
        isEmbedded = true;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;

        transform.position = landPoint + landNormal * 0.05f;
        transform.up = landNormal;

        mainCollider.isTrigger = true;

        if (canPickupFromGround)
        {
            EnablePickup();
        }
    }

    #endregion

    #region Sistema de Recolección

    private void EnablePickup()
    {
        isAvailableForPickup = true;

        if (pickupIndicator != null)
        {
            pickupIndicator.SetActive(true);
        }
    }

    private void DisablePickup()
    {
        isAvailableForPickup = false;

        if (pickupIndicator != null)
        {
            pickupIndicator.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isAvailableForPickup || !other.CompareTag(playerTag)) return;

        // Verificar si podemos recogerlo según el estado
        bool canPickup = false;

        if (currentState == HarpoonState.EmbeddedInGround && canPickupFromGround)
            canPickup = true;
        else if (currentState == HarpoonState.EmbeddedInEnemy && canPickupFromEnemy)
            canPickup = true;
        else if (currentState == HarpoonState.Falling && canPickupFromGround)
            canPickup = true;

        if (canPickup)
        {
            CollectHarpoon();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        // Feedback visual opcional cuando el jugador está cerca
        if (isAvailableForPickup && other.CompareTag(playerTag))
        {
            // Aquí podrías agregar efectos visuales, como escalar el indicador
            PulsePickupIndicator();
        }
    }

    private void CollectHarpoon()
    {
        CancelInvoke();
        DisablePickup();

        if (stuckParent != null)
        {
            if (stuckEnemy != null)
                stuckEnemy.OnHarpoonRemoved(this);

            transform.SetParent(null);
        }

        ReturnToPool();
    }

    private void PulsePickupIndicator()
    {
        if (pickupIndicator != null && pickupIndicator.activeSelf)
        {
            float scale = 1f + Mathf.Sin(Time.time * indicatorPulseSpeed) * 0.2f;
            pickupIndicator.transform.localScale = Vector3.one * scale;
        }
    }

    #endregion

    #region Pool Management

    private void BreakHarpoon()
    {
        // Aquí puedes agregar efectos de ruptura (partículas, sonido)
        Debug.Log($"Arpón destruido contra {breakTags[0]}");
        ReturnToPool();
    }

    private void ReturnToPool()
    {
        DisablePickup();

        if (stuckParent != null)
        {
            if (stuckEnemy != null)
                stuckEnemy.OnHarpoonRemoved(this);

            transform.SetParent(null);
        }

        pool?.ReturnToPool(gameObject);
    }

    #endregion

    #region Helpers

    private bool IsBreakTag(string tagToCheck)
    {
        foreach (string t in breakTags)
            if (t == tagToCheck) return true;
        return false;
    }

    private bool IsGroundLayer(int layer)
    {
        return ((1 << layer) & groundLayers.value) != 0;
    }

    private void AlignWithVelocity()
    {
        if (rb.linearVelocity.magnitude > 0.1f)
            transform.forward = rb.linearVelocity.normalized;
    }

    #endregion

    #region Physics Update

    private void FixedUpdate()
    {
        if (currentState == HarpoonState.Flying)
        {
            HandleFlyingPhysics();
        }
    }

    private void HandleFlyingPhysics()
    {
        // Aplicar gravedad
        if (rb.linearVelocity.y > -18f)
            rb.AddForce(Physics.gravity * gravityScale, ForceMode.Acceleration);

        // Alinear con la velocidad
        AlignWithVelocity();

        // Detección anticipada
        PredictiveCollisionCheck();
    }

    private void PredictiveCollisionCheck()
    {
        if (rb.linearVelocity.magnitude <= 8f) return;

        float checkDistance = rb.linearVelocity.magnitude * Time.fixedDeltaTime + 0.4f;

        if (Physics.Raycast(transform.position, rb.linearVelocity.normalized,
            out RaycastHit hit, checkDistance, groundLayers))
        {
            EmbedInGround(hit.point, hit.normal);
        }
    }

    #endregion

    #region Debug

    private void OnDrawGizmosSelected()
    {
        // Mostrar radio de recolección
        Gizmos.color = isAvailableForPickup ? Color.green : Color.yellow;
        Gizmos.DrawWireSphere(transform.position, pickupRadius);

        // Mostrar estado
        if (Application.isPlaying)
        {
            Gizmos.color = currentState switch
            {
                HarpoonState.Flying => Color.cyan,
                HarpoonState.EmbeddedInGround => Color.green,
                HarpoonState.EmbeddedInEnemy => Color.red,
                HarpoonState.Falling => Color.yellow,
                _ => Color.white
            };
            Gizmos.DrawSphere(transform.position, 0.1f);
        }
    }

    #endregion
}