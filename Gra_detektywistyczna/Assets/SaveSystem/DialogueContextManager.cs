using System.Text;
using UnityEngine;

public class DialogueContextManager : MonoBehaviour
{
    public static DialogueContextManager Instance { get; private set; }
    public static StringBuilder GameContext { get; private set; } = new StringBuilder();
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public static void AddPlayerDialogue(string playerName, string dialogue)
    {
        GameContext.AppendLine($"{playerName.ToUpper()}:");
        GameContext.AppendLine($"{dialogue}\n");
    }
    
    public static void AddNPCDialogue(string npcName, string response)
    {
        GameContext.AppendLine("ODP:");
        GameContext.AppendLine($"{response}\n");
    }
    
    public static void AddSceneChange(string sceneName, string sceneDescription)
    {
        GameContext.AppendLine("ZMIANA SCENY:");
        GameContext.AppendLine($"{sceneName}");
        GameContext.AppendLine($"{sceneDescription}\n");
    }
    
    public static void ClearContext()
    {
        GameContext.Clear();
    }
    
    public static string GetFormattedContext()
    {
        return GameContext.ToString();
    }

    public static void SetContext(string loadedHistory)
    {
        GameContext.Clear();
        GameContext.Append(loadedHistory);
    }
}