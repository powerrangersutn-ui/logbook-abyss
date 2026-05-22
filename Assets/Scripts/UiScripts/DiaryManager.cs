using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DiaryManager : MonoBehaviour
{
    public static DiaryManager Instance; // Singleton para acceso global

    [Header("Referencias UI")]
    [SerializeField] private GameObject messagePanel;

    [SerializeField] private TextMeshProUGUI messageText;

    [Tooltip("Arrastrar los GameObjects de los botones Bottle(1) al (8) EN ORDEN")]
    [SerializeField] private GameObject[] bottleButtons;

    private int bottlesFound = 0;

    // Diccionario con los comunicados. La clave (int) es el número de botella.
    private Dictionary<int, string> bottleMessages = new Dictionary<int, string>()
    {
        { 1, "Hemos explorado las profundidades mapeadas para corroborar que son habitables para el Proyecto. La exploración duró 8 días y los resultados son los siguientes:\r\nSuelo: apto para construcción. \r\nProfundidad: ideal para construcción de elevador planificada con tanque de óxigeno para el Proyecto.\r\n Distancia a tierra firme: 180km más lejana a la mínima estimada. \r\nFauna: animales pequeños transitorios. No se encontraron hogares a 50km a la redonda. \r\nFlora: Se realizaron experimentos con cada biotipo encontrado para corroborar sus pro y contras, principal objetivo: verificar si son transplantables a ambiente artificial. \r\nObservaciones: A 10km del perímetro, ahora cercado, para llevar a cabo el Proyecto, se encuentra una zona rocosa, con terreno inestable, huecos, muy oscuro, y no pudimos penetrarlo. No sabemos qué recursos y/o peligros habrá, se espera descubrirlo en próximas espediciones.\r\nEsperamos que este solo sea el comienzo de un cambio necesario para nuestro mundo actual." },
        { 2, "Empezamos con la construcción de la base. El suelo responde bien, los obreros trabajan a buen ritmo, los materiales efectivamente son duraderos. El equipo de investigación pudo corroborar que la flora puede adaptarse en cautiverio, aunque el perímetro sí parece ser una ruta importante para la fauna. \r\nA corroborar cómo podemos solventar este problema. Deberíamos ir a ver el terreno mencionado anteriormente, ya que periódicamente aparecen hordas de peces, como si escaparan de ese sector. \r\nEs importante investigar para ver si algún peligro acecha esa zona." },
        { 3, "Hace ya 6 meses hicimos nuestra primera expedición y la base ya está casi terminada. \r\nHace dos meses que los investigadores asignados viven dentro, todos los estudios médicos van bien, su alimentación y ejercicio es igual al de la superficie y muestran signos de contento con la base. \r\nAlgunas de las hordas de peces provenientes del terreno desconocido han chocado fuertemente con la base. Los materiales son resistentes pero algunas marcas quedan. Parecen totalmente asustados. \r\nUna vez más, dejo constatado lo menester de investigar esta zona, podríamos enfrentarnos a indeseados en un futuro no tan lejano." },
        { 4, "A un año de nuestra primera exploración, el Proyecto ya ha entrado en la fase más importante: testear cómo funciona la economía dentro de la base. \r\nLa gente que ahora mora allí parece adaptarse fácilmente y con entusiasmo. La economía funciona, por el momento, según lo esperado. \r\nLo que me preocupa: hace un mes salió una exploración a las tierras peligrosas, llamadas de ahora en más “El Vacío” según artículo 345.3, apartado 245 de El Proyecto. \r\nHemos tenido contacto con ellos hasta un día después de su partida. Después ya no conseguimos comunicarnos. Estamos en proceso de investigación para la segunda expedición: armas, tanques de óxigeno y linternas serán necesarias." },
        { 5, "La expedición 13 fue, como las otras, perdida en El Vacío. La gente empieza a cuestionarse por qué salen tantos submarinos de la base, cada vez más modernizados, pero nunca vuelven. \r\nLa mentira de que estamos construyendo otra base en otros lugares ya no la creen, ni que los peces que chocan contra la base siguen vivos, cuando realmente pusimos trampas para comerlos y alimentarnos, ya que están demorándose los envíos desde la superficie. \r\nSé que el artículo 1.1 habla de confidencialidad en estos comunicados, pero necesitamos plantear maneras nuevas de manipular a las masas y poder seguir explotando estos terrenos para las próximas construcciones... pero sobre todo, necesitamos descrubir qué sucede en El Vacío."},
        { 6, "Con un cálido saludo les escribe Comunicador 2. \r\nComunicador 1 ha sido envíado a la expedición 15 de El Vacío por su gran valor y dedicación al Proyecto. Tristemente, tampoco ha vuelto esa expedición. \r\nDatos recolectados: mientras más te adentrás, más oscuro es. Fauna desconocida pero de fuertes ataques. Sonidos extraños y sombríos, podrían ser paralizantes para los exploradores. Esta expedición en específica parece haber llegado a un lugar clave gracias a una comunicación importante... con poca información. \r\nRastrearemos en el mapa los pasos dados para llegar al punto y descifrar realmente qué esconden estas aguas. \r\nPor otro lado, la gente que vive en la base se encuentra un tanto... insatisfecha. Algunos pudieron ser legalmente llevados a la superficie, otros quisieron escapar y fallecieron en el intento. ¡Pero que no ceda el optimismo! Hay un grupo de gente vive feliz y con ganas de procrear dentro de la base. El Proyecto sigue su rumbo gracias a ellos, y a ustedes, por supuesto."},
        { 7, "No sé si este comunicado llegará pero lo intentaré. Hemos sufrido un ataque. No sabemos de qué, pero fue fuerte. No fue un barco, fue un monstruo. Bueno, eso creo, porque se movía como un ser vivo gigante. Fue a altas horas de la noche, lo que me hace suponer que nadie lo vio más que el Centro de Control. La gente fue apaciguada diciendo que fue una ballena, pero sabemos que no lo fue. Espero no estar quebrando las reglas de confidencialidad con este mensaje pero es menester que podamos acabar con la amenaza del vacío... o abortar misión."},
        { 8, "Comunicador 2 aquí. Luego de mi último mensaje a La Gerencia, me mandaron a la expedición 17. Debí haber sido más cuidadoso, debí haber seguido las reglas. \r\nMás allá de mi desgracia, he podido encontrar la evidencia que buscábamos, y me temo que es peor de lo que esperábamos: no solamente hay peces humanoides, o del estilo “Sirena”, sino peces híbridos de las más peligrosas especies, huecos que son un pase gratuito a tu muerte, y oscuridad absoluta. \r\nGracias a expediciones anteriores pude encontrar tanques de oxígeno que me permitieron llegar aquí, y gracias a la luz que creó el Centro de Investigación pude realizar el camino un poco más valiente. Pero la verdadera desgracia no es todo esto, esto es solo contexto. \r\nLa verdadera desgracia es el monstruo (no le gusta ser llamado asì). No me dijo su nombre, pero es sumamente enorme, casi tan enorme como El Vacío mismo. Los peces que creíamos que escapaban, no lo hacían: atacaban a La Base porque, efectivamente, era su hogar, y además conectaba con la criatura que resultó ser un ser de unión y protector de la fauna de esta zona. \r\nNo entiendo qué vio en mí para contarme esto, pero hay una sola cosa de la que tengo certeza: no me dejará salir de aquí. Su sed de venganza ante los humanos por destruir su habitat es insaciable, y por eso nadie volvió, ni volverá.\r\nSi estás leyendo esto, quiere decir que vos tampoco. Intentá volver con todas tus fuerzas, realmente espero que puedas hacerlo. \r\nYo cumpliré mi condena por ser cómplice de el Proyecto. Ahí viene..."}
    };

    private void Awake()
    {
        Instance = this;

        // Al arrancar, nos aseguramos de que todos los botones y el panel de mensaje estén ocultos
        foreach (GameObject btn in bottleButtons)
        {
            btn.SetActive(false);
        }
        messagePanel.SetActive(false);
    }

    /// Llamado por las botellas del mapa al ser agarradas.
    public void UnlockNextBottle()
    {
        bottlesFound++;

        if (bottlesFound <= bottleButtons.Length)
        {
            bottleButtons[bottlesFound - 1].SetActive(true); //-1 ya que los array arrancan en 0
        }
    }

    /// Llamado por los botones desde el Inspector (OnClick).
    public void ShowMessage(int bottleNumber)
    {
        if (bottleMessages.ContainsKey(bottleNumber))
        {
            messagePanel.SetActive(true);
            messageText.text = bottleMessages[bottleNumber];
        }
    }
}