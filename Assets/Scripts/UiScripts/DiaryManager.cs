using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DiaryManager : MonoBehaviour
{
    public static DiaryManager Instance;

    public static event Action OnBottleFound;
    public static event Action OnLogbookFound;

    [Header("Referencias UI")]
    [SerializeField] private GameObject messagePanel;
    [SerializeField] private TextMeshProUGUI messageText;

    [Tooltip("Arrastrar los componentes Button de los botones Bottle(1) al (8)")]
    [SerializeField] private Button[] bottleButtons;

    [Tooltip("Arrastrar el botón correspondiente a la bitácora")]
    [SerializeField] private Button logbookButton;

    [Header("Feedback UI")]
    [SerializeField] private GameObject bottleInteractionFeedback;

    private int bottlesFound = 0;

    // Diccionario con los comunicados. La clave es el número de botella.
    private Dictionary<string, string> bottleMessages = new Dictionary<string, string>()
    {
        {"1", "Hemos explorado las profundidades mapeadas para corroborar que son habitables para el Proyecto. La exploración duró 8 días y los resultados son los siguientes:\r\n\r\nSuelo: apto para construcción. \r\n\r\nProfundidad: ideal para construcción de elevador planificada con tanque de óxigeno para el Proyecto.\r\n\r\nDistancia a tierra firme: 180km más lejana a la mínima estimada. \r\n\r\nFauna: animales pequeños transitorios. No se encontraron hogares a 50km a la redonda. \r\n\r\nFlora: se realizaron experimentos con cada biotipo encontrado para corroborar sus pro y contras, principal objetivo: verificar si son transplantables a ambiente artificial. \r\n\r\nObservaciones: A 10km del perímetro, ahora cercado, para llevar a cabo el Proyecto, se encuentra una zona rocosa, con terreno inestable, huecos, muy oscuro, y no pudimos penetrarlo. No sabemos qué recursos y/o peligros habrá, se espera descubrirlo en próximas espediciones.\r\n\r\n\r\nEsperamos que este solo sea el comienzo de un cambio necesario para nuestro mundo actual." },
        {"2", "Empezamos con la construcción de la base. El suelo responde bien, los obreros trabajan a buen ritmo, los materiales efectivamente son duraderos. El equipo de investigación pudo corroborar que la flora puede adaptarse en cautiverio, aunque el perímetro sí parece ser una ruta importante para la fauna. \r\n\r\nA corroborar cómo podemos solventar este problema. Deberíamos ir a ver el terreno mencionado anteriormente, ya que periódicamente aparecen hordas de peces, como si escaparan de ese sector. \r\n\r\nEs importante investigar para ver si algún peligro acecha esa zona." },
        {"3", "Hace ya 6 meses hicimos nuestra primera expedición y la base ya está casi terminada. \r\n\r\nHace dos meses que los investigadores asignados viven dentro, todos los estudios médicos van bien, su alimentación y ejercicio es igual al de la superficie y muestran signos de contento con la base. \r\n\r\nAlgunas de las hordas de peces provenientes del terreno desconocido han chocado fuertemente con la base. Los materiales son resistentes pero algunas marcas quedan. Parecen totalmente asustados. \r\n\r\nUna vez más, dejo constatado lo menester de investigar esta zona, podríamos enfrentarnos a indeseados en un futuro no tan lejano." },
        {"4", "A un año de nuestra primera exploración, el Proyecto ya ha entrado en la fase más importante: testear cómo funciona la economía dentro de la base. \r\n\r\nLa gente que ahora mora allí parece adaptarse fácilmente y con entusiasmo. La economía funciona, por el momento, según lo esperado. \r\n\r\nLo que me preocupa: hace un mes salió una exploración a las tierras peligrosas, llamadas de ahora en más “El Vacío” según artículo 345.3, apartado 245 de El Proyecto. \r\n\r\nHemos tenido contacto con ellos hasta un día después de su partida. Después ya no conseguimos comunicarnos. Estamos en proceso de investigación para la segunda expedición: armas, tanques de óxigeno y linternas serán necesarias." },
        {"5", "La expedición 13 fue, como las otras, perdida en El Vacío. La gente empieza a cuestionarse por qué salen tantos submarinos de la base, cada vez más modernizados, pero nunca vuelven. \r\n\r\nLa mentira de que estamos construyendo otra base en otros lugares ya no la creen, ni que los peces que chocan contra la base siguen vivos, cuando realmente pusimos trampas para comerlos y alimentarnos, ya que están demorándose los envíos desde la superficie. \r\n\r\nSé que el artículo 1.1 habla de confidencialidad en estos comunicados, pero necesitamos plantear maneras nuevas de manipular a las masas y poder seguir explotando estos terrenos para las próximas construcciones... pero sobre todo, necesitamos descubrir qué sucede en El Vacío."},
        {"6", "Con un cálido saludo les escribe Comunicador 2. \r\n\r\nComunicador 1 ha sido envíado a la expedición 15 de El Vacío por su gran valor y dedicación al Proyecto. Tristemente, tampoco ha vuelto esa expedición. \r\n\r\n\r\nDatos recolectados: mientras más te adentrás, más oscuro es. Fauna desconocida pero de fuertes ataques. Sonidos extraños y sombríos, podrían ser paralizantes para los exploradores. Esta expedición en específica parece haber llegado a un lugar clave gracias a una comunicación importante... con poca información. \r\n\r\nRastrearemos en el mapa los pasos dados para llegar al punto y descifrar realmente qué esconden estas aguas. \r\n\r\nPor otro lado, la gente que vive en la base se encuentra un tanto... insatisfecha. Algunos pudieron ser legalmente llevados a la superficie, otros quisieron escapar y fallecieron en el intento. ¡Pero que no ceda el optimismo! Hay un grupo de gente vive feliz y con ganas de procrear dentro de la base. El Proyecto sigue su rumbo gracias a ellos, y a ustedes, por supuesto."},
        {"7", "No sé si este comunicado llegará pero lo intentaré. Hemos sufrido un ataque. No sabemos de qué, pero fue fuerte. No fue un barco, fue un monstruo. Bueno, eso creo, porque se movía como un ser vivo gigante. Fue a altas horas de la noche, lo que me hace suponer que nadie lo vio más que el Centro de Control. La gente fue apaciguada diciendo que fue una ballena, pero sabemos que no lo fue. \r\n\r\nEspero no estar quebrando las reglas de confidencialidad con este mensaje pero es menester que podamos acabar con la amenaza del vacío... o abortar misión."},
        {"logbook", "Comunicador 2 aquí. Luego de mi último mensaje a La Gerencia, me mandaron a la expedición 17. Debí haber sido más cuidadoso, debí haber seguido las reglas. \r\n\r\nMás allá de mi desgracia, he podido encontrar la evidencia que buscábamos, y me temo que es peor de lo que esperábamos: no solamente hay peces humanoides, o del estilo “Sirena”, sino peces híbridos de las más peligrosas especies, huecos que son un pase gratuito a tu muerte, y oscuridad absoluta. \r\n\r\nGracias a expediciones anteriores pude encontrar tanques de oxígeno que me permitieron llegar aquí, y gracias a la luz que creó el Centro de Investigación pude realizar el camino un poco más valiente. Pero la verdadera desgracia no es todo esto, esto es solo contexto. La verdadera desgracia es el monstruo (no le gusta ser llamado asì). No me dijo su nombre, pero es sumamente enorme, casi tan enorme como El Vacío mismo. Los peces que creíamos que escapaban, no lo hacían: atacaban a La Base porque, efectivamente, era su hogar, y además conectaba con la criatura que resultó ser un ser de unión y protector de la fauna de esta zona. \r\n\r\nNo entiendo qué vio en mí para contarme esto, pero hay una sola cosa de la que tengo certeza: no me dejará salir de aquí. Su sed de venganza ante los humanos por destruir su habitat es insaciable, y por eso nadie volvió, ni volverá.\r\n\r\nSi estás leyendo esto, quiere decir que vos tampoco. Intentá volver con todas tus fuerzas, realmente espero que puedas hacerlo. \r\n\r\nYo cumpliré mi condena por ser cómplice de el Proyecto. Ahí viene..."}
    };
    //private void Start() 
    //{
    //    Instance = this;

    //    if (messagePanel != null) messagePanel.SetActive(false);

    //    if (bottleButtons != null)
    //    {
    //        foreach (Button btn in bottleButtons)
    //        {
    //            if (btn != null)
    //            {
    //                btn.interactable = false;
    //                Debug.Log("Bloqueando botón: " + btn.name);
    //            }
    //        }
    //    }
    //}

    private void Awake()
    {
        Instance = this;

        foreach (Button btn in bottleButtons) btn.interactable = false;
        if (logbookButton != null) logbookButton.interactable = false;

        messagePanel.SetActive(false);
    }

    private void OnEnable()
    {
        OnBottleFound += UnlockNextBottle;
        OnLogbookFound += UnlockLogbookButton;
    }

    private void OnDisable()
    {
        OnBottleFound -= UnlockNextBottle;
        OnLogbookFound -= UnlockLogbookButton;
    }

    public static void RaiseBottleFound() => OnBottleFound?.Invoke();
    public static void RaiseLogbookFound() => OnLogbookFound?.Invoke();

    /// Llamado por las botellas del mapa al ser agarradas

    public void UnlockNextBottle()
    {
        if (bottlesFound < bottleButtons.Length)
        {
            bottleButtons[bottlesFound].interactable = true;

            bottlesFound++;

            if (bottleInteractionFeedback != null)
            {
                bottleInteractionFeedback.SetActive(true);
                StartCoroutine(HideFeedbackAfterDelay(3f));
            }
        }
    }

    public void UnlockLogbookButton()
    {
        if (logbookButton != null)
        {
            logbookButton.interactable = true;
        }

        if (bottleInteractionFeedback != null)
        {
            bottleInteractionFeedback.SetActive(true);
            StartCoroutine(HideFeedbackAfterDelay(3f));
        }
    }

    private System.Collections.IEnumerator HideFeedbackAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        bottleInteractionFeedback.SetActive(false);
    }



    public void ShowMessage(string messageKey)
    {
        if (bottleMessages.ContainsKey(messageKey))
        {
            messagePanel.SetActive(true);

            if (messageText != null)
            {
                messageText.text = bottleMessages[messageKey];
                messageText.ForceMeshUpdate();
            }
            else
            {
                Debug.LogError("messageText no está asignado en el Inspector de DiaryManager");
            }
        }
    }

    public void HideMessagePanel()
    {
        if (messagePanel != null)
        {
            messagePanel.SetActive(false);
        }
    }
}