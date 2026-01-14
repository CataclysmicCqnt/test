using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections;
using DTOModel;
using System.Collections.Generic;
using TMPro;
using System.Threading.Tasks;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }
    [Header("UI References")]
    [SerializeField] private Image NpcImage;

    public string sceneContext;
    public string currentNpcName;
    public bool isFreeDiscussionEnabled;

    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;
    public GameObject dialoguePanel;
    public TMP_InputField inputField;

    public List<Dialogue> dialoguesHistory;
    private Queue<Dialogue> dialoguesQueue;
    public bool isAwaitingUserInput = false;
    public bool isAwaitingNPCResponse = false;

    async void Start()
    {
        string history = DialogueContextManager.GetFormattedContext();
        Debug.Log($"kontekst: {!string.IsNullOrEmpty(history)}");

        sceneContext = "Scenariusz: " + GameSession.CurrentScenarioName;

        if (!string.IsNullOrEmpty(history))
        {
            if (DialogueEngineManager.Instance == null || !DialogueEngineManager.IsInitialized)
            {
                Debug.LogWarning("DialogueEngine nie jest jeszcze zainicjalizowany. Pomijam GenerateNewSceneAsync na starcie.");
                return;
            }

            SceneDTO context = new SceneDTO
            {
                LocationName = GameSession.CurrentScenarioName,
                ScenePrompt = "To jest kontynuacja śledztwa. Historia do tej pory: " + history
            };

            await DialogueEngineManager.Instance.GenerateNewSceneAsync(context);
        }
    }

    
    void Awake()
    {
        Instance = this;
        dialoguesQueue = new Queue<Dialogue>();
        dialoguesHistory = new List<Dialogue>();
    }

    void Update()
    {
        if (Keyboard.current.enterKey.wasPressedThisFrame && !isAwaitingNPCResponse)
        {
            if (isAwaitingUserInput)
            {
                var userText = inputField != null ? inputField.text : string.Empty;

                dialoguesHistory.Add(new Dialogue("Ty", userText));

                NPCRequestDTO npcRequestDTO = new NPCRequestDTO()
                {
                    SceneDescription = sceneContext,
                    UserText = userText,
                    NPCName = currentNpcName,
                };

                isAwaitingUserInput = false;
                isAwaitingNPCResponse = true;

                if (nameText != null) nameText.text = "Narrator";
                DisableUserInput();
                ShowLoadingResponse();

                SendNpcRequest(npcRequestDTO);
            }
            else
            {
                PlayDialogue();
            }
        }
    }

    public void AskQuestion(string name)
    {
        
        Dialogue dialogue = new Dialogue("Ty", sceneContext);
        dialogue.isPlayerPrompt = true;
        EnqueueDialogue(dialogue);
        EnqueueDialogue(new Dialogue(name, ""));
    }

    public void EnqueueDialogue(Dialogue dialogue)
    {
        dialoguesQueue.Enqueue(dialogue);
    }

    public void PlayDialogue()
    {
        ShowDialogue();
        StopAllCoroutines();
        if (dialoguesQueue.Count == 0)
        {
            EndDialogue();
            return;
        }
        Dialogue dialog = dialoguesQueue.Dequeue();
        if (dialog.isPlayerPrompt)
        {
            PromptPlayerAsync();
        }
        else
        {
            DisplayDialogue(dialog);
        }
    }
    public async Task PromptPlayerAsync()
    {
        HideDialogueText();

        if (dialoguesQueue == null || dialoguesQueue.Count == 0)
        {
            EndDialogue();
            return;
        }

        currentNpcName = dialoguesQueue.Peek().name;

        //zmienianie sprite npc (nie wiem czy w dobrym miejscu najwyzej poprawie)
        
        string[] parameters = { MenuControl.CurrentScenarioName,MenuControl.CurrentSceneNumber.ToString()};
        SceneScriptDTO scene = await DialogueEngineManager.Instance.GetSceneAsync(parameters);
        foreach (NPCDTO npc in scene.Npcs)
        {
            if (currentNpcName == npc.name.Split(' ')[0])
            {
                Sprite newSprite = Resources.Load<Sprite>(currentNpcName);
                NpcImage.sprite = newSprite;
                if (NpcImage.color.a == 0f)
                {
                    NpcImage.color = Color.white;
                }
            }
        }
        nameText.text = "Ty";
        EnableUserInput();
        isAwaitingUserInput = true;
    }

    public async void SendNpcRequest(NPCRequestDTO npcRequestDTO)
    {
        if (npcRequestDTO == null)
        {
            isAwaitingNPCResponse = false;
            StopAllCoroutines();
            EndDialogue();
            return;
        }

        if (DialogueEngineManager.Instance == null || !DialogueEngineManager.IsInitialized)
        {
            isAwaitingNPCResponse = false;
            StopAllCoroutines();
            EndDialogue();
            return;
        }

        if (dialoguesQueue == null || dialoguesQueue.Count == 0)
        {
            isAwaitingNPCResponse = false;
            StopAllCoroutines();
            EndDialogue();
            return;
        }

        DialogueContextManager.AddPlayerDialogue("Ty", npcRequestDTO.UserText);

        NPCResponseDTO response = null;

        try
        {
            response = await DialogueEngineManager.Instance.AskNPCAsync(npcRequestDTO);
        }
        catch (System.Exception ex)
        {
            Debug.LogException(ex);
            isAwaitingNPCResponse = false;
            StopAllCoroutines();
            EndDialogue();
            return;
        }

        if (response == null || string.IsNullOrEmpty(response.Speech))
        {
            isAwaitingNPCResponse = false;
            StopAllCoroutines();
            EndDialogue();
            return;
        }

        dialoguesQueue.Peek().sentence = response.Speech;
        dialoguesQueue.Peek().name = currentNpcName;

        DialogueContextManager.AddNPCDialogue(currentNpcName, response.Speech);

        PlayDialogue();
    }

    public void DisplayDialogue(Dialogue dialogue)
    {
        dialoguesHistory.Add(dialogue);
        currentNpcName = dialogue.name;
        dialogueText.text = dialogue.sentence;
        nameText.text = dialogue.name;
        StartCoroutine(TypeSentence(dialogue.sentence));
    }

    IEnumerator TypeSentence(string sentence)
    {
        dialogueText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return null;
        }
    }

    void EndDialogue()
    {
        if (isFreeDiscussionEnabled)
        {
            AskQuestion(currentNpcName);
            PlayDialogue();
        }
        else
        {
            HideDialogue();
        }
    }

    IEnumerator AnimateTypingDots()
    {
        string baseText = currentNpcName + " myśli";
        int dotCount = 0;

        while (true)
        {
            dialogueText.text = baseText + new string('.', dotCount % 4);
            dotCount++;
            yield return new WaitForSeconds(0.5f);
        }
    }

    public void ShowLoadingResponse()
    {
        ShowDialogue();
        StartCoroutine(AnimateTypingDots());
    }

    public void ShowDialogue()
    {
        if (dialoguePanel != null)
            dialoguePanel.SetActive(true);
    }

    public void HideDialogueText()
    {
        if (dialogueText != null)
            dialogueText.text = "";
    }

    public void HideDialogue()
    {
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);
    }

    public void Reset()
    {
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);
    }

    public void EnableUserInput()
    {
        if (inputField == null) return;

        inputField.gameObject.SetActive(true);
        inputField.interactable = true;
        inputField.ActivateInputField();
    }

    public void DisableUserInput()
    {
        if (inputField == null) return;

        inputField.text = "";
        inputField.gameObject.SetActive(false);
        inputField.interactable = false;
        inputField.DeactivateInputField();
    }
}
