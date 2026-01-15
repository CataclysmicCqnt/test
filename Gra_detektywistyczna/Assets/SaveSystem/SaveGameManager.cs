using DTOModel;
using System;
using System.Threading.Tasks;
using UnityEngine;

public class SaveGameManager : MonoBehaviour
{
    public static SaveGameManager Instance { get; private set; }
    
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
    
    public async Task<bool> SaveGameAsync(string gameTitle)
    {
        try
        {
            CreatedGameDTO newGame = new CreatedGameDTO
            {
                Title = GameSession.CurrentScenarioName,
                LastSaveDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                CurrentSceneNumber = GameSession.CurrentSceneNumber,
                MaxSceneNumber = GameSession.MaxSceneNumber,
                GameHistory = DialogueContextManager.GetFormattedContext()
            };
            
            await DialogueEngineManager.Instance.SaveGameAsync(newGame);
            
            Debug.Log($"Zapisano: {gameTitle} (Scena {newGame.CurrentSceneNumber})");
            Debug.Log($"Historia: {newGame.GameHistory.Length} znaków");
            
            return true;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Błąd zapisu: {ex.Message}");
            return false;
        }
    }
   

}