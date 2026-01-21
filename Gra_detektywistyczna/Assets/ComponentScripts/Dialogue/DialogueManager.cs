using DTOModel;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }
    [Header("UI References")]
    [SerializeField] private Image NpcImage;

    public string currentNpcName;
    public bool isFreeDiscussionEnabled;

    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;
    public GameObject dialoguePanel;
    public TMP_InputField inputField;

    public List<Dialogue> dialoguesHistory;
    private Queue<Dialogue> dialoguesQueue;
    private bool isAwaitingUserInput = false;
    private bool isAwaitingNPCResponse = false;

    private bool _sceneLoadedOnServer = false;
    private Task _sceneLoadTask;

    async void Start()
    {
        NewSceneService.View = this;

        if (!string.IsNullOrEmpty(GameSession.PendingVerdictText))
        {
            ShowDialogue();
            StopAllCoroutines();

            EnqueueDialogue(new Dialogue(string.IsNullOrEmpty(GameSession.PendingVerdictNpcName) ? "AI" : GameSession.PendingVerdictNpcName, GameSession.PendingVerdictText));

            GameSession.PendingVerdictText = null;

            GameSession.PendingVerdictNpcName = null; ;

            PlayDialogue();

            return;
        }

        string history = DialogueContextManager.GetFormattedContext();
        Debug.Log($"kontekst: {!string.IsNullOrEmpty(history)}");

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
                ScenePrompt = GameSession.CurrentScene.Description + "To jest kontynuacja śledztwa. Historia do tej pory: " + history
            };
            Debug.Log(await DialogueEngineManager.Instance.GenerateNewSceneAsync(context));
        }
        else
        {
            Clear();
            foreach (var character in GameSession.CurrentScene.Npcs)
            {
                EnqueueDialogue(
                    new Dialogue(
                        "Narrator",
                        "Podchodzisz do " + character.name + ", możesz zadać 3 pytania."
                    )
                );

                AskQuestion(character.name);
                AskQuestion(character.name);
                AskQuestion(character.name);
            }

            PlayDialogue();
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
                    SceneDescription = GameSession.CurrentScene.Description,
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
    public async void LoadNewScene()
    {
        await MenuControl.Instance.NextScene();
        Clear();
        foreach (var character in GameSession.CurrentScene.Npcs)
        {
            EnqueueDialogue(
                new Dialogue(
                    "Narrator",
                    "Podchodzisz do " + character.name + ", możesz zadać 3 pytania."
                )
            );

            AskQuestion(character.name);
            AskQuestion(character.name);
            AskQuestion(character.name);
        }

        PlayDialogue();

        await Task.Yield(); // UI ma szansę się odbudować

        if (!inputField)
            return;

        EventSystem.current.SetSelectedGameObject(inputField.gameObject);
        inputField.ActivateInputField();
        inputField.Select();
    }

    public async void LoadCurrentScene()
    {
        Clear();
        foreach (var character in GameSession.CurrentScene.Npcs)
        {
            EnqueueDialogue(
                new Dialogue(
                    "Narrator",
                    "Podchodzisz do " + character.name + ", możesz zadać 3 pytania."
                )
            );

            AskQuestion(character.name);
            AskQuestion(character.name);
            AskQuestion(character.name);
        }

        PlayDialogue();

        await Task.Yield(); // UI ma szansę się odbudować

        if (!inputField)
            return;

        EventSystem.current.SetSelectedGameObject(inputField.gameObject);
        inputField.ActivateInputField();
        inputField.Select();
    }

    public void LoadNpcTexture(string npcName)
    {
        NpcImage.sprite = Resources.Load<Sprite>(npcName);
        if (NpcImage.color.a == 0f)
        {
            NpcImage.color = Color.white;
        }
    }
    public void AskQuestion(string name)
    {
        Dialogue dialogue = new Dialogue("Ty", "");
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
            PromptPlayer();
        }
        else
        {
            DisplayDialogue(dialog);
        }
    }
    public void PromptPlayer()
    {
        HideDialogueText();

        if (dialoguesQueue == null || dialoguesQueue.Count == 0)
        {
            EndDialogue();
            return;
        }

        currentNpcName = dialoguesQueue.Peek().name;
        if (currentNpcName != "Narrator") LoadNpcTexture(currentNpcName);
        Debug.Log(currentNpcName);
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
            await EnsureSceneLoadedOnServerAsync();
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
        isAwaitingNPCResponse = false;
    }

    public void DisplayDialogue(Dialogue dialogue)
    {
        dialoguesHistory.Add(dialogue);
        currentNpcName = dialogue.name;
        if (currentNpcName != "Narrator") LoadNpcTexture(currentNpcName);
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
    public void Clear()
    {
        isAwaitingNPCResponse = false;
        isAwaitingUserInput = false;
        dialoguesHistory.Clear();
        dialoguesQueue.Clear();
        HideDialogue();
        DisableUserInput();
        currentNpcName = string.Empty;
        NpcImage.sprite = null;
        NpcImage.color = new Color(NpcImage.color.r, NpcImage.color.g, NpcImage.color.b, 0f);

    }

    private async Task EnsureSceneLoadedOnServerAsync()
    {
        if (_sceneLoadedOnServer) return;

        if (_sceneLoadTask != null)
        {
            await _sceneLoadTask;
            return;
        }

        SceneDTO sceneDTO = new SceneDTO
        {
            LocationName = GameSession.CurrentScenarioName,
            ScenePrompt = "Scenariusz: " + GameSession.CurrentScenarioName
        };

        _sceneLoadTask = DialogueEngineManager.Instance.GenerateNewSceneAsync(sceneDTO);
        await _sceneLoadTask;

        _sceneLoadedOnServer = true;
    }
}
