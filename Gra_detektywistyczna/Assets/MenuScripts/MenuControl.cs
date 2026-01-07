using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using DTOModel;
using System.Collections.Generic;
using System;

public class MenuControl : MonoBehaviour
{
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
        DialogueContextManager.ClearContext();
        string[] parameters = { "scenerio_apollo", "1"};
        int maxScene = 12;

        GameSession.StartSession(parameters[0], int.Parse(parameters[1]), maxScene);
        
        SceneScriptDTO scene = await DialogueEngineManager.Instance.GetSceneAsync(parameters);
        SceneManager.LoadScene("NewGame");
    }

    public async void NextScene()
    {
        string[] parameters = { GameSession.CurrentScenarioName, (GameSession.CurrentSceneNumber + 1).ToString() };
        SceneScriptDTO scene = await DialogueEngineManager.Instance.GetSceneAsync(parameters);
        if (scene != null)
        {
            GameSession.CurrentSceneNumber++;
            Debug.Log($"Next scene: {GameSession.CurrentSceneNumber}");
        }
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
