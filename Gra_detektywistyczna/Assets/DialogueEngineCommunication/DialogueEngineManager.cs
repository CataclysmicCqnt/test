using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using DTOModel;
using DTOModel;
using NUnit.Framework;
using UnityEngine;
using System.Linq;
public class DialogueEngineManager
{
    private static string exeRelativePath = "DialogueEngine/DialogueEngine/bin/Debug/net8.0/DialogueEngine.exe";
    private DialogueEngineClient _client;

    public static DialogueEngineManager Instance { get; private set; }
    public static bool IsInitialized { get; private set; }

    public DialogueEngineManager(DialogueEngineClient client)
    {
        _client = client;
    }

    public static async Task InitializeManagerAsync(GameObject gameObject)
    {
        if (Instance != null)
        {
            return;
        }

        Debug.Log("Inicjalizacja DialogueEngine!");

        string exePath = Application.dataPath + "/../../" + exeRelativePath;

        DialogueEngineClient client = new DialogueEngineClient(exePath);

        Instance = new DialogueEngineManager(client);
        await Instance.InitializeEngineAsync();
    }

    public async Task QuitManagerAsync()
    {
        if (_client != null)
        {
            Debug.Log("Zamkni�cie DialogueEngine!");
            _client.ForceKill();
            _client.Dispose();
        }
    }

    public async Task InitializeEngineAsync()
    {
        try
        {
            await _client.InitializeAsync();
            IsInitialized = true;
            Debug.Log("Zainicjalizowano DialogueEngine");
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Blad inicjalizacji DialogueEngine: " + ex.Message);
        }
    }

    public async Task<GamesToContinueDTO> GetGamesToContinueAsync()
    {
        if (IsInitialized == false)
            throw new System.Exception("Jeszcze nie zainicjalizowano DialogueEngine!");
        return await _client.GetGamesToContinueAsync();
    }

    public async Task<SceneScriptDTO> GetSceneAsync(string[] parameters)
    {
        if (IsInitialized == false)
            throw new System.Exception("Jeszcze nie zainicjalizowano DialogueEngine!");
        return await _client.GetSceneAsync(parameters);
    }

    public async Task<SettingsDTO> GetSettingsAsync()
    {
        if (IsInitialized == false)
            throw new System.Exception("Jeszcze nie zainicjalizowano DialogueEngine!");
        return await _client.GetSettingsAsync();
    }

    public async Task SaveSettingsAsync(SettingsDTO settingsDTO)
    {
        if (IsInitialized == false)
            throw new System.Exception("Jeszcze nie zainicjalizowano DialogueEngine!");
        await _client.SaveSettingsAsync(settingsDTO);
    }

    public async Task<NPCResponseDTO> AskNPCAsync(NPCRequestDTO requestDTO)
    {
        if (IsInitialized == false)
            throw new System.Exception("Jeszcze nie zainicjalizowano DialogueEngine!");
        return await _client.AskNPCAsync(requestDTO);
    }

    public async Task SaveGameAsync(CreatedGameDTO gameDTO)
    {
        if (IsInitialized == false)
            throw new System.Exception("Jeszcze nie zainicjalizowano DialogueEngine!");
        
        await _client.SaveGameAsync(gameDTO);
    }

    public async Task<string> GenerateNewSceneAsync(SceneDTO sceneDTO)
    {
        if (IsInitialized == false)
            throw new System.Exception("Jeszcze nie zainicjalizowano DialogueEngine!");

        return await _client.GenerateNewSceneAsync(sceneDTO);
    }

    public async Task<string> GetRandomScenarioAsync()
    {
        if (IsInitialized == false)
            throw new System.Exception("Jeszcze nie zainicjalizowano DialogueEngine!");
        return await _client.GetRandomScenarioAsync();
    }
    public async Task<bool> DeleteGameAsync(CreatedGameDTO gameToDelete)
    {
        string filePath = Path.Combine(Application.dataPath, "Database", "SavedGames.json");

        return await Task.Run(() =>
        {
            try
            {
                if (!File.Exists(filePath)) return false;

                string jsonContent = File.ReadAllText(filePath);
                GamesToContinueDTO db = JsonUtility.FromJson<GamesToContinueDTO>(jsonContent);

                if (db == null || db.GamesToContinue == null) return false;

                // --- POPRAWKA TUTAJ ---
                // 1. Zamieniamy tablicę na Listę (dzięki System.Linq)
                List<CreatedGameDTO> gamesList = db.GamesToContinue.ToList();

                // 2. Usuwamy element z Listy
                int removedCount = gamesList.RemoveAll(g =>
                    g.Title == gameToDelete.Title &&
                    g.LastSaveDate == gameToDelete.LastSaveDate
                );

                if (removedCount > 0)
                {
                    // 3. Zamieniamy Listę z powrotem na tablicę (wymagane przez Twoje DTO)
                    db.GamesToContinue = gamesList.ToArray();

                    // 4. Zapisujemy zmiany
                    string newJson = JsonUtility.ToJson(db, true);
                    File.WriteAllText(filePath, newJson);

                    Debug.Log($"Usunięto grę: {gameToDelete.Title}");
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Błąd: {e.Message}");
                return false;
            }
        });
    }
}
