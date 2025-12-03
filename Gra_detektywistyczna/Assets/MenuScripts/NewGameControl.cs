using DTOModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.MenuScripts
{
    public class NewGameControl : MonoBehaviour
    {
        private async void Awake()
        {
            Debug.Log("New Game Loaded");
        }

        public AudioClip sound;

        void Start()
        {
            string context = "Wchodząc do Galerii Apollo, od razu czujesz, że coś nie tak. Światło z neonowych świateł rozprasza się w rozbitych kawałkach szkła, które rozrzucone są po podłodze niczym ślady po niebezpiecznej grze. Na środku sali, w miejscu, gdzie zwykle wystawiałby się najcenniejszy obraz, leży rozbita gablota, a w powietrzu unosi się zapach spalenizny i stęchlizny. \r\n\r\nWśród zniszczeń stoją trzy postacie: \r\n- **Joanne** – elegancka, ale zdezorientowana właścicielka galerii, z rękoma zaciśniętymi w dłonie, próbująca zrozumieć, co się stało. \r\n- **Darmian** – młody, zwinny sprzedawca sztuki, którego oczy błyszczą niepokojem; wciąż trzyma w ręku zniszczony szkic, który miał być kluczem do rozwiązania zagadki. \r\n- **Casper** – tajemniczy kolekcjoner, którego twarz jest częściowo zasłonięta maską, a w kieszeni trzyma mały, metalowy klucz, który zdaje się pasować do jakiegoś ukrytego mechanizmu w galerii.\r\n\r\nWśród rozrzuconych przedmiotów znajdują się dwa interaktywne elementy:\r\n1. **Rozbity szkic** – w nim zapisane są fragmenty rysunku, które po odpowiednim ułożeniu mogą wskazać ukryte drzwi. \r\n2. **Metalowy klucz** – wydaje się pasować do zamka w jednej z zniszczonych ścian; jego użycie może odsłonić tajne przejście.\r\n\r\nGracz musi szybko zdecydować, komu zaufać i które przedmioty wykorzystać, aby odkryć, kto stoi za kradzieżą i rozbiciem gabloty. Każda decyzja może prowadzić do nieoczekiwanego zwrotu akcji w tej pełnej napięcia scenie.";
            DialogueManager.Instance.sceneContext = context;
            DialogueManager.Instance.EnqueueDialogue(new Dialogue("Narrator", "Wchodząc do Galerii Apollo, od razu czujesz, że coś nie tak. Światło z neonowych świateł rozprasza się w rozbitych kawałkach szkła, które rozrzucone są po podłodze niczym ślady po niebezpiecznej grze."));
            DialogueManager.Instance.EnqueueDialogue(new Dialogue("Narrator", "Na środku sali, w miejscu, gdzie zwykle wystawiałby się najcenniejszy obraz, leży rozbita gablota, a w powietrzu unosi się zapach spalenizny i stęchlizny."));
            DialogueManager.Instance.EnqueueDialogue(new Dialogue("Narrator", "Pochodzisz do kobiety, możesz zadać jej 3 pytania."));
            DialogueManager.Instance.AskQuestion("Joanne");
            DialogueManager.Instance.AskQuestion("Joanne");
            DialogueManager.Instance.AskQuestion("Joanne");
            DialogueManager.Instance.EnqueueDialogue(new Dialogue("Narrator", "Pochodzisz do mężczyzny, możesz zadać mu 3 pytania."));
            DialogueManager.Instance.AskQuestion("Darmian");
            DialogueManager.Instance.AskQuestion("Darmian");
            DialogueManager.Instance.AskQuestion("Darmian");
            DialogueManager.Instance.PlayDialogue();
            AudioManager.Instance.PlaySFX(sound);
        }
    }
}
