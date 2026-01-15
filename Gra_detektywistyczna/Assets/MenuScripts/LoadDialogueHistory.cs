using TMPro;
using UnityEngine;
using DTOModel;

public class LoadDialogueHistory : MonoBehaviour
{
    public TMP_Text myText;   // Podpinamy TextMeshPro w Inspectorze
    private CreatedGameDTO save;

    // OnEnable jest wywo³ywane za ka¿dym razem, gdy obiekt lub jego rodzic jest aktywny
    void OnEnable()
    {
        // Sprawdzamy, czy panel nadrzêdny jest aktywny w hierarchii
        if (transform.parent != null && transform.parent.gameObject.activeInHierarchy)
        {
            save = CurrentSaveManager.Instance.GetCurrentSave();

            if (save != null)
            {
                Debug.Log("Uda³o siê dostaæ aktualny zapis");
                SetText(save.GameHistory);
                Debug.Log("Uda³o siê wypisaæ tekst na ekran");
            }
            else
            {
                Debug.LogWarning("Brak aktualnego zapisu!");
            }
        }
        else
        {
            Debug.Log($"{gameObject.name} nie uruchomione, bo panel nieaktywny");
        }
    }

    // Metoda do ustawiania tekstu w TMP
    public void SetText(string newText)
    {
        if (myText != null)
            myText.text = newText;
        else
            Debug.LogWarning("Nie przypisano TMP_Text!");
    }
}

