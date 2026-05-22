using UnityEngine;

public class BottlePickup : MonoBehaviour, IInteractable
{
    // Booleano de seguridad para evitar doble recolección
    private bool wasPickedUp = false;

    public void OnInteract(PlayerInventory inventory)
    {
        if (wasPickedUp) return;
        wasPickedUp = true;

        Debug.Log("[BottlePickup] ¡Botella interactuada y recogida!");

        // Le avisamos al Manager que encontramos UNA botella más
        if (DiaryManager.Instance != null)
        {
            DiaryManager.Instance.UnlockNextBottle();
        }
        else
        {
            Debug.LogWarning("[BottlePickup] Ojo: No hay un DiaryManager en la escena.");
        }

        // Destruimos la botella del mapa (le dejamos un mini delay por seguridad visual/sonora si tuvieras)
        Destroy(gameObject, 0.1f);
    }
}