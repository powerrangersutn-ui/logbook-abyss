using UnityEngine;
using System;
using System.Collections;

public class BoxPickup : MonoBehaviour, IInteractable
{
    [Header("Cantidades de Oxígeno")]
    [SerializeField] private float minOxygen = 18f;
    [SerializeField] private float maxOxygen = 35f;

    [Header("Probabilidades de Arpones")]
    [Range(0, 100)][SerializeField] private float chance1Harpoon = 50f;
    [Range(0, 100)][SerializeField] private float chance2Harpoons = 35f;

    [Header("Probabilidad general")]
    [Range(0, 100)][SerializeField] private float harpoonChanceWhenNormal = 80f;

    [Header("Sounds")]
    [SerializeField] private float volume = 3f;
    [SerializeField] private AudioSource playerPickAudio;
    [SerializeField] private AudioClip pickupSound;

    private bool wasPickedUp = false;

    [SerializeField] private float uiShowDistance = 3f;
    [SerializeField] private GameObject interactionCanvas;

    private Transform playerTransform;

    public event Action OnOpen;
    [Header("Animation Settings")]
    [SerializeField] private float timeBeforeDestroy = 5f;

    private void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) playerTransform = player.transform;
    }

    private void Update()
    {
        if (wasPickedUp || interactionCanvas == null)
        {
            if (interactionCanvas != null) interactionCanvas.SetActive(false);
            return;
        }

        if (playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) playerTransform = player.transform;
            return;
        }

        float distance = Vector3.Distance(transform.position, playerTransform.position);
        interactionCanvas.SetActive(distance <= uiShowDistance);
    }

    public void OnInteract(PlayerInventory inventory)
    {
        if (wasPickedUp) return;
        wasPickedUp = true;

        OnOpen?.Invoke();

        if (inventory.GetComponent<OxygenSystem>().IsLowOxygen)
        {
            GiveOxygen(inventory);
        }
        else
        {
            if (UnityEngine.Random.value * 100f <= harpoonChanceWhenNormal && inventory.CanReceiveHarpoons())
            {
                GiveHarpoons(inventory);
            }
            else
            {
                GiveOxygen(inventory);
            }
        }

        if (interactionCanvas != null) interactionCanvas.SetActive(false);

        playerPickAudio.PlayOneShot(pickupSound, volume);

        StartCoroutine(DestroyAfterAnimation());
    }

    private IEnumerator DestroyAfterAnimation()
    {
        yield return new WaitForSeconds(timeBeforeDestroy);
        Destroy(gameObject);
    }

    private void GiveOxygen(PlayerInventory inventory)
    {
        float amount = UnityEngine.Random.Range(minOxygen, maxOxygen);
        inventory.AddOxygen(amount);
    }

    private void GiveHarpoons(PlayerInventory inventory)
    {
        int amount = GetHarpoonAmountByProbability();
        int given = inventory.AddHarpoons(amount);

        if (given > 0)
        {
            ;
        }
    }

    private int GetHarpoonAmountByProbability()
    {
        float roll = UnityEngine.Random.value * 100f;
        float cumulative = chance1Harpoon;

        if (roll <= cumulative) return 1;
        cumulative += chance2Harpoons;
        if (roll <= cumulative) return 2;
        return 3;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, uiShowDistance);
    }
}