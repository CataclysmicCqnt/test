using UnityEngine;
using DTOModel;

public class CurrentSaveManager : MonoBehaviour
{
    public static CurrentSaveManager Instance;


    private CreatedGameDTO currentSave;
    public static bool isNewGame = true;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }


    public void SetCurrentSave(CreatedGameDTO save)
    {
        if (save == null)
        {
            Debug.LogWarning("Pr�ba ustawienia null save'a!");
            return;
        }

        currentSave = save;
        Debug.Log("Aktualny save zosta� ustawiony!");
    }


    public CreatedGameDTO GetCurrentSave()
    {
        if (currentSave == null)
        {
            Debug.LogWarning("Brak aktualnego save'a!");
        }

        return currentSave;
    }
    public void SetIsNewGame(bool isNew)
    {
        isNewGame = isNew;
    }
    public bool IsNewGame()
    {
        return isNewGame;
    }
}