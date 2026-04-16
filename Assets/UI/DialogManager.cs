using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;

public class DialogManager : MonoBehaviour
{
    public static DialogManager Instance; // Singleton para fácil acceso
    [SerializeField] GameObject dialogPanel;    
    [SerializeField] TextMeshProUGUI textDialog;   
    [SerializeField] float timeBetweenLines = 2.5f;

    private Dictionary<string, string[]> dialogData = new Dictionary<string, string[]>() {
        { "hint_intro", new string[] { "Recuerda tu objetivo: buscar la bitácora en el submarino. Es de suma importancia que la traigas de nuevo a tu punto de salida." }},
        { "hint_hole", new string[] { "Recuerda que este terreno es como una montaña: si te caes mueres." }},
        { "hint_fightEnemy", new string[] { "Hey, puedo ver en tu radar que hay alguna criatura peligrosa. Puedes usar tu arpón para acabarla."
                                           ,"Apunta bien: estas flechas son frágiles pero poderosas. Si le pegas a algún objeto duro seguro se romperán."
                                           ,"Presiona la barra espaciadora para disparar. Apunta con tu mouse."}},
        { "hint_goal", new string[] { "Atención! el submarino está cerca. Tu objetivo se encuentra dentro." }},
        { "hint_logbook", new string[] { "Eureka!!! Ya tienes la bitácora. Ahora apúrate, debes volver al ascensor antes de que te agarren esas escorias."}},
        { "hint_elevator", new string[] { "El ascensor está cerca!"}},
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