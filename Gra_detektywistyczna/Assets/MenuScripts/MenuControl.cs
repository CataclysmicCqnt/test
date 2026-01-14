using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DTOModel;
using System.Collections.Generic;
using System;

public class MenuControl : MonoBehaviour
{
    public static Dictionary<string, string> CollectedCharacters = new Dictionary<string, string>();

    public static MenuControl Instance;
    public static int CurrentSceneNumber { get; private set; }
    public static string CurrentScenarioName { get; private set; }

    [Header("UI References")]
    [SerializeField] private Image backgroundImage;

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
        string[] parameters = { "scenerio_apollo", "1" };

        CurrentScenarioName = parameters[0];
        CurrentSceneNumber = int.Parse(parameters[1]);


        SceneScriptDTO scene = await DialogueEngineManager.Instance.GetSceneAsync(parameters);

        if (scene != null)
        {

            MenuControl.CollectedCharacters.Clear();


            foreach (var character in scene.Npcs)
            {
                if (!MenuControl.CollectedCharacters.ContainsKey(character.name))
                {
                    MenuControl.CollectedCharacters.Add(character.name, character.protrait);
                    Debug.Log($"Dodano do słownika: {character.name} ze ścieżką {character.protrait}");
                }
            }


            SceneManager.LoadScene("NewGame");
        }

    }

    public async void NextScene()
    {
        string[] parameters = { CurrentScenarioName, (++CurrentSceneNumber).ToString() };
        SceneScriptDTO scene = await DialogueEngineManager.Instance.GetSceneAsync(parameters);


        if (scene != null)
        {

            Debug.Log("Nazwa tla to: " + scene.Background);


            SetBackground(scene.Background);

            Debug.Log("Next scene: " + CurrentSceneNumber);
        }
        else
        {
            Debug.Log("Wykryto koniec scenariusza (null). Zmieniam scenę na podsumowanie.");

            UnityEngine.SceneManagement.SceneManager.LoadScene("EndGameSummary");
        }
    }

    private void SetBackground(string backgroundName)
    {

        Sprite newSprite = Resources.Load<Sprite>(backgroundName);
        if (backgroundImage == null)
        {
            return;
        }

        if (newSprite != null)
        {
            backgroundImage.sprite = newSprite;
        }
        else
        {
            Debug.LogError($"Nie znaleziono grafiki o nazwie: {backgroundName} w Resources");
        }
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
