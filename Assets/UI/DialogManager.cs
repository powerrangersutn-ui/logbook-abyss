using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogManager : MonoBehaviour
{
    public static DialogManager Instance; // Singleton para fácil acceso
    [SerializeField] GameObject dialogPanel;    
    [SerializeField] TextMeshProUGUI textDialog;   
    [SerializeField] float timeBetweenLines = 2.5f;

    private Dictionary<string, string[]> dialogData = new Dictionary<string, string[]>() {
        { "npc_saludo", new string[] { "¡Hola, viajero!"
                                      ,"Hacía mucho que no veía a alguien por aquí."
                                      ,"¿Buscas algo en particular?" }},
        { "item_espada", new string[] { "Es una espada vieja..."
                                       ,"Parece que aún tiene filo."
                                       ,"Podría ser útil." }}
    };

    void Awake() { Instance = this; }

    public void ShowDialog(string refHint)
    {
        if (dialogData.ContainsKey(refHint))
        {
            StopAllCoroutines();
            StartCoroutine(SequenceDialog(dialogData[refHint]));
        }
    }

    IEnumerator SequenceDialog(string[] lines)
    {
        dialogPanel.SetActive(true);

        foreach (string line in lines)
        {
            textDialog.text = line;
            yield return new WaitForSeconds(timeBetweenLines);
        }

        dialogPanel.SetActive(false);
        textDialog.text = "";
    }
}