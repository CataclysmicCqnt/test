using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections;
using DTOModel;
using System.Collections.Generic;
using TMPro;
using UnityEngine.EventSystems;
public
class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

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
	
	private bool isDialogueFullyPrinted = false;

    async void Start()
    {
        string history = DialogueContextManager.GetFormattedContext();
        Debug.Log($"kontekst: {!string.IsNullOrEmpty(history)}");

        sceneContext = "Scenariusz: " + GameSession.CurrentScenarioName;

        if (!string.IsNullOrEmpty(history))
        {
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
        if (Keyboard.current.enterKey.wasPressedThisFrame)
        {
            if (isAwaitingNPCResponse)
            {
                return;
            }
            if (isAwaitingUserInput)
            {
                HandlePlayerInputSubmission();
            }
            else if (isDialogueFullyPrinted)
            {
                PlayDialogue();
            }
        }
    }
	private void HandlePlayerInputSubmission()
    {
        this.enabled = false;
        isAwaitingNPCResponse = true;

        isAwaitingUserInput = false;
        isAwaitingNPCResponse = true;

        string userMessage = inputField.text;

        NPCRequestDTO npcRequestDTO = new NPCRequestDTO()
        {
            SceneDescription = sceneContext,
            UserText = userMessage,
            NPCName = currentNpcName,
        };

        dialoguesHistory.Add(new Dialogue("Ty", userMessage));
        nameText.text = "Narrator";

        DisableUserInput();

        ShowLoadingResponse();

        SendNpcRequest(npcRequestDTO);
    }
	public void DisableUserInput()
    {
        inputField.text = "";
        inputField.interactable = false;
        inputField.DeactivateInputField();
        inputField.gameObject.SetActive(false);

        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
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
        currentNpcName = dialoguesQueue.Peek().name;
        nameText.text = "Ty";
        EnableUserInput();
        isAwaitingUserInput = true;
    }
    public async void SendNpcRequest(NPCRequestDTO npcRequestDTO)
    {
        isAwaitingNPCResponse = true;

        Debug.Log("[AI] Rozpoczynam wysyłanie...");
        DialogueContextManager.AddPlayerDialogue("Ty", npcRequestDTO.UserText);

        try
        {
            await System.Threading.Tasks.Task.Delay(100);

            NPCResponseDTO response = await DialogueEngineManager.Instance.AskNPCAsync(npcRequestDTO);

            Debug.Log("[AI] Przyszła odpowiedź!");

            if (dialoguesQueue.Count > 0)
            {
                dialoguesQueue.Peek().sentence = response.Speech;
                dialoguesQueue.Peek().name = currentNpcName;
            }
            DialogueContextManager.AddNPCDialogue(currentNpcName, response.Speech);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[AI ERROR] {e.Message}");
        }
        finally
        {
            Debug.Log("[AI] Koniec operacji - ODBLOKOWUJĘ Enter.");
            StopAllCoroutines();

            isAwaitingNPCResponse = false;
            this.enabled = true;
            PlayDialogue();
        }
    }
    public void DisplayDialogue(Dialogue dialogue)
    {
        dialoguesHistory.Add(dialogue);
        currentNpcName = dialogue.name;
        dialogueText.text = dialogue.sentence;
        nameText.text = dialogue.name;
		isDialogueFullyPrinted = false;
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
		isDialogueFullyPrinted = true;
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
        dialoguePanel.SetActive(true);
    }
    public void HideDialogueText()
    {
        dialogueText.text = "";
    }
    public void HideDialogue()
    {
        dialoguePanel.SetActive(false);
    }
    public void Reset()
    {
        dialoguePanel.SetActive(false);
    }
    public void EnableUserInput()
    {
        inputField.gameObject.SetActive(true);
        inputField.interactable = true;
        inputField.ActivateInputField();
    }
}