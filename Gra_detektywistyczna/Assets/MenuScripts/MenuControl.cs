using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using DTOModel;
using System.Collections.Generic;
using System;

public class MenuControl : MonoBehaviour
{
    public static int CurrentSceneNumber { get; private set; }
    public static string CurrentScenarioName { get; private set; }

    private async void Awake()
    {
        await DialogueEngineManager.InitializeManagerAsync(gameObject);
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

    public void BackToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public async void NewGameScene()
    {
        string[] parameters = { "scenerio_apollo", "1"};

        CurrentScenarioName = parameters[0];
        CurrentSceneNumber = int.Parse(parameters[1]);
        
        SceneScriptDTO scene = await DialogueEngineManager.Instance.GetSceneAsync(parameters);
        SceneManager.LoadScene("NewGame");
    }

    public async void NextScene()
    {
        string[] parameters = { CurrentScenarioName, (++CurrentSceneNumber).ToString() };
        SceneScriptDTO scene = await DialogueEngineManager.Instance.GetSceneAsync(parameters);
        if (scene != null) Debug.Log("Next scene");
    }

    public void ContinueGameScene()
    {
        SceneManager.LoadScene("ContinueGame");
    }

    public void  AuthorsScene()
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
