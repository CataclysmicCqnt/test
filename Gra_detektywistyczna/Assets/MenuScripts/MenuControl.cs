using DTOModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;


public class MenuControl : MonoBehaviour
{
    public static Dictionary<string, string> CollectedCharacters = new Dictionary<string, string>();

    public static MenuControl Instance;
    public static int CurrentSceneNumber { get; private set; }  //uzywac GameSession.CurrentSceneNumber
    public static string CurrentScenarioName { get; private set; }  //uzywac GameSession.CurrentScenarioName

    [Header("UI References")]
    [SerializeField] private Image backgroundImage;

    private async void Awake()
    {
        await DialogueEngineManager.InitializeManagerAsync(gameObject);
        Instance = this;
        BackgroundService.View = this;
        DontDestroyOnLoad(gameObject);
    }

    private async void OnApplicationQuit()
    {
        if (DialogueEngineManager.Instance != null)
        {
            await DialogueEngineManager.Instance.QuitManagerAsync();
        }
    }

    public MenuControl()
    {
        Debug.Log("Start programu!");
    }

    private void Clean()
    {
        Debug.Log("Clean sesji i menuControl");
        CollectedCharacters = new Dictionary<string, string>();
        CurrentSceneNumber = 0;
        CurrentScenarioName = string.Empty;

        GameSession.CleanSession();
    }

    public void BackToMenu()
    {
        Clean();

        SceneManager.LoadScene("MainMenu");
    }

    public async void NewGameScene()
    { 
        DialogueContextManager.ClearContext();
    
        string scenarioName = GetRandomScenarioFromFolder();

        Debug.Log($"Używam scenariusza: {scenarioName}");

        string[] parameters = { scenarioName, "1" };
        SceneScriptDTO scene = await DialogueEngineManager.Instance.GetSceneAsync(parameters);


        if (scene != null)
        {
            GameSession.StartSession(parameters[0], int.Parse(parameters[1]), scene);
            SceneDTO context = new SceneDTO
            {
                LocationName = GameSession.CurrentScenarioName,
                ScenePrompt = GameSession.CurrentScene.Description
            };
            Debug.Log(await DialogueEngineManager.Instance.GenerateNewSceneAsync(context));
            MenuControl.CollectedCharacters.Clear();
            foreach (var character in scene.Npcs)
            {
                if (!MenuControl.CollectedCharacters.ContainsKey(character.name))
                {
                    MenuControl.CollectedCharacters.Add(character.name, character.protrait);
                }
            }

            SceneManager.LoadScene("NewGame");
        }
        else
        {
            Debug.LogError($"Nie można załadować scenariusza: {scenarioName}");
            SceneManager.LoadScene("MainMenu");
        }
    }

    private string GetRandomScenarioFromFolder()
    {
        string databasePath = Application.dataPath + "/Database/";

        if (!System.IO.Directory.Exists(databasePath))
        {
            Debug.LogError("Brak folderu Database!");
            return "scenerio_apollo";
        }

        var files = System.IO.Directory.GetFiles(databasePath, "*.json")
            .Select(f => System.IO.Path.GetFileNameWithoutExtension(f))
            .Where(f => f != "Settings" && f != "SavedGames")
            .ToList();

        if (files.Count == 0)
        {
            return "scenerio_apollo";
        }

        System.Random random = new System.Random();
        return files[random.Next(0, files.Count)];
    }

    public async Task NextScene()
    {
        string[] parameters = { GameSession.CurrentScenarioName, (GameSession.CurrentSceneNumber + 1).ToString() };
        SceneScriptDTO scene = await DialogueEngineManager.Instance.GetSceneAsync(parameters);
        


        if (scene != null)
        {

            Debug.Log("Nazwa tla to: " + scene.Background);


            BackgroundService.SetBackground(scene.Background);

            GameSession.StartSession(GameSession.CurrentScenarioName, GameSession.CurrentSceneNumber+1, scene);
            SceneDTO context = new SceneDTO
            {
                LocationName = GameSession.CurrentScenarioName,
                ScenePrompt = GameSession.CurrentScene.Description
            };
            Debug.Log(await DialogueEngineManager.Instance.GenerateNewSceneAsync(context));
            foreach (var character in scene.Npcs)
            {
                if (!MenuControl.CollectedCharacters.ContainsKey(character.name))
                {
                    MenuControl.CollectedCharacters.Add(character.name, character.protrait);
                }
            }
            Debug.Log("Next scene: " + GameSession.CurrentSceneNumber);
        }
        else
        {
            Debug.Log("Wykryto koniec scenariusza (null). Zmieniam scenę na podsumowanie.");

            UnityEngine.SceneManagement.SceneManager.LoadScene("EndGameSummary");
        }
    }

    public static async Task CurrentScene()
    {
        string[] parameters = { GameSession.CurrentScenarioName, GameSession.CurrentSceneNumber.ToString() };
        SceneScriptDTO scene = await DialogueEngineManager.Instance.GetSceneAsync(parameters);

        if (scene != null)
        {

            Debug.Log("Nazwa tla to: " + scene.Background);


            BackgroundService.SetBackground(scene.Background);

            GameSession.StartSession(GameSession.CurrentScenarioName, GameSession.CurrentSceneNumber + 1, scene);
            SceneDTO context = new SceneDTO
            {
                LocationName = GameSession.CurrentScenarioName,
                ScenePrompt = GameSession.CurrentScene.Description
            };
            Debug.Log(await DialogueEngineManager.Instance.GenerateNewSceneAsync(context));
            foreach (var character in scene.Npcs)
            {
                if (!MenuControl.CollectedCharacters.ContainsKey(character.name))
                {
                    MenuControl.CollectedCharacters.Add(character.name, character.protrait);
                }
            }
            Debug.Log("Next scene: " + GameSession.CurrentSceneNumber);
        }
        else
        {
            Debug.Log("Wykryto koniec scenariusza (null). Zmieniam scenę na podsumowanie.");

            UnityEngine.SceneManagement.SceneManager.LoadScene("EndGameSummary");
            return;
        }
    }

    public void SetBackground(Sprite sprite)
    {
        if (!backgroundImage || !sprite)
            return;

        backgroundImage.sprite = sprite;
    }


    public void ContinueGameScene()
    {
        SceneManager.LoadScene("ContinueGame");
    }

    public void AuthorsScene()
    {
        SceneManager.LoadScene("Authors");
    }

    public void SettingsScene()
    {
        SceneManager.LoadScene("Settings");
    }

    public void QuickGame()
    {
        Application.Quit();
#if UNITY_EDITOR

        UnityEditor.EditorApplication.isPlaying = false;
#endif

    }
}
