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

            NPCRequestDTO npcRequestDTO = new NPCRequestDTO()
            {
                SceneDescription = "Rozbita gablota w Galerii Apollo. Wokół stoją Joanne, Darmian, Casper i Serena. Chaos po kradzieży",
                UserText = "Przedstaw się",
                NPCName = "Joanne Galtier",
            };
            Debug.Log("New Game Scene Started");
            NPCResponseDTO response = await DialogueEngineManager.Instance.AskNPCAsync(npcRequestDTO);
            Dialogue dialogue = new Dialogue("John", response.Speech );

            DialogueManager.Instance.EnqueueDialogue(dialogue);
            DialogueManager.Instance.AskQuestion();
            DialogueManager.Instance.AskQuestion();
            npcRequestDTO = new NPCRequestDTO()
            {
                SceneDescription = "Rozbita gablota w Galerii Apollo. Wokół stoją Joanne, Darmian, Casper i Serena. Chaos po kradzieży",
                UserText = "Przedstaw się",
                NPCName = "Darmian Duchamp",
            };
            response = await DialogueEngineManager.Instance.AskNPCAsync(npcRequestDTO);
            dialogue = new Dialogue("Marek", response.Speech);
            DialogueManager.Instance.EnqueueDialogue(dialogue);
            DialogueManager.Instance.AskQuestion();
            DialogueManager.Instance.AskQuestion();
            DialogueManager.Instance.PlayDialogue();
        }

        public AudioClip sound;

        void Start()
        {
            AudioManager.Instance.PlaySFX(sound);
        }
    }
}
