using UnityEngine;
using TMPro;

public class AmmoHUDController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI ammoText;

    // Cantidad cargada en el arma (será 1 o 0)
    private int loadedAmount;

    public void UpdateAmmo(bool isLoaded, int inventoryAmount)
    {
        if (isLoaded)
        {
            loadedAmount = 1;
        }
        else
        {
            loadedAmount = 0;
        }

        int remainingInventory = inventoryAmount - loadedAmount;

        ammoText.text = loadedAmount.ToString() + " / " + remainingInventory.ToString();
    }
}