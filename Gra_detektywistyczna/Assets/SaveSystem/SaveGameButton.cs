using UnityEngine;
using UnityEngine.UI;

public class SaveGameButton : MonoBehaviour
{
    [SerializeField] private Button saveButton;
    
    private void Awake()
    {
        saveButton.onClick.AddListener(OnSaveButtonClicked);
    }
    
    private async void OnSaveButtonClicked()
    {
        Debug.Log("Kliknieto ZAPISZ!");
        
        string saveName = "Zapis_" + System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm");
        
        if (SaveGameManager.Instance != null)
        {
            bool success = await SaveGameManager.Instance.SaveGameAsync(saveName);
            Debug.Log(success ? "Zapisano: " + saveName : "Blad zapisu!");
        }
    }
}