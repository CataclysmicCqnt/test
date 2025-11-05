using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuControl : MonoBehaviour
{
    public void BackToMenu()
    {
        SceneManager.LoadScene("MainMenu");

    }

    public void NewGameScene()
    {
        SceneManager.LoadScene("NewGame");
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
