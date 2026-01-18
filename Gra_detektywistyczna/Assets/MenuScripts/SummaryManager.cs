using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;
using System.Threading.Tasks;

public class SummaryManager : MonoBehaviour
{
    [Header("Konfiguracja UI")]
    public RectTransform canvasRect;
    public GameObject cardTemplate;

    void Start()
    {
        if (MenuControl.CollectedCharacters == null || MenuControl.CollectedCharacters.Count == 0)
        {
            Debug.LogWarning("SummaryManager: Brak zebranych postaci.");
            return;
        }
        DisplayCharacters();
    }

    private void DisplayCharacters()
    {
        var characters = MenuControl.CollectedCharacters;
        float totalWidth = canvasRect.rect.width;
        float spacing = totalWidth / (characters.Count + 1);

        int i = 1;
        foreach (var entry in characters)
        {
            GameObject newCard = Instantiate(cardTemplate, canvasRect);
            newCard.SetActive(true);
            newCard.transform.SetAsLastSibling();

            RectTransform rt = newCard.GetComponent<RectTransform>();
            float posX = (-totalWidth / 2f) + (spacing * i);
            rt.anchoredPosition = new Vector2(posX, 0);

            TextMeshProUGUI txt = newCard.GetComponentInChildren<TextMeshProUGUI>();
            if (txt != null)
            {
                string fullName = entry.Key;
                string firstName = fullName.Split(' ')[0];
                txt.text = firstName;
                txt.raycastTarget = false;
            }

                Image img = newCard.GetComponent<Image>();
            if (img != null)
            {
                img.raycastTarget = false;

                string path = !string.IsNullOrEmpty(entry.Value) ? entry.Value : entry.Key;
                Sprite s = Resources.Load<Sprite>(path);
                if (s != null) img.sprite = s;
                else Debug.LogError($"[SummaryManager] Brak pliku '{path}' w Resources!");
            }

            Button btn = newCard.GetComponentInChildren<Button>();
            if (btn != null)
            {
                string charName = entry.Key;
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() => OnCardClicked(charName));

                Image btnImg = btn.GetComponent<Image>();
                if (btnImg != null) btnImg.raycastTarget = true;
            }
            else
            {
                Debug.LogError($"[SummaryManager] Nie znaleziono przycisku wewn?trz karty {entry.Key}!");
            }

            i++;
        }
        cardTemplate.SetActive(false);
    }

    public async Task OnCardClicked(string name)
    {
        Debug.Log("Klikniêta postaæ: " + name);

        string verdict = await DialogueEngineManager.Instance.GetNpcVerdictAsync(name);

        if (string.IsNullOrEmpty(verdict))
        {
            Debug.LogError("Nie uda³o siê pobraæ podsumowania z AI!");
            return;
        }

        GameSession.PendingVerdictNpcName = name;
        GameSession.PendingVerdictText = verdict;

        SceneManager.LoadScene("EmptyScene");
    }

}