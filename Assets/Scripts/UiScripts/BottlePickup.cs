using UnityEngine;

public class BottlePickup : MonoBehaviour, IInteractable
{
    // Booleano de seguridad para evitar doble recolección
    private bool wasPickedUp = false;

    public void OnInteract(PlayerInventory inventory)
    {
        if (wasPickedUp) return;
        wasPickedUp = true;

        DiaryManager.RaiseBottleFound();

        Destroy(gameObject, 0.1f);
    }
}