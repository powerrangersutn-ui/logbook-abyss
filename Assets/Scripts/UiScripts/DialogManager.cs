using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogManager : MonoBehaviour
{
    public static DialogManager Instance; // Singleton para fácil acceso
    [SerializeField] GameObject dialogPanel;    
    [SerializeField] TextMeshProUGUI textDialog;   
    [SerializeField] float timeBetweenLines = 3f;

    private Dictionary<string, string[]> dialogData = new Dictionary<string, string[]>() {
        { "hint_intro", new string[] { "Recuerda tu objetivo: buscar la bitácora en el submarino. Es de suma importancia que la traigas de nuevo a tu punto de salida."
                                           ,"Cuida tu oxígeno, correr y saltar hará que se acabe más rápido."}},
        { "hint_hole", new string[] { "Recuerda que este terreno es como una montaña: si te caes mueres."
                                           ,"Puedo ver que hay algunas botellas con información. Por favor recoléctalas para nuestras documentaciones"}},
        { "hint_fightEnemy", new string[] { "Hey, puedo ver en tu radar que hay alguna criatura peligrosa. Puedes usar tu arpón para acabarla."
                                           ,"Apunta bien: estas flechas son frágiles pero poderosas. Si le pegas a algún objeto duro seguro se romperán."
                                           ,"Presiona la e o click izquierdo para disparar. Apunta con tu mouse."
                                           ,"Según nuestros informes debería haber cajas de expediciones anteriores con recursos como tanques de oxígeno y flechas. Aprovechalas"}},
        { "hint_goal", new string[] { "Atención! el submarino está cerca. Tu objetivo se encuentra dentro." }},
        { "hint_logbook", new string[] { "Ahora que tenés la bitácora, apurate en volver! Estoy detectando una criatura colosal acercandose!"}},
        { "hint_elevator", new string[] { "El ascensor está cerca! Ya casi estás!"}},
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