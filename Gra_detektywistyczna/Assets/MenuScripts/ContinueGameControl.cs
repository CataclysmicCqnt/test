﻿using DTOModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;


namespace Assets.MenuScripts
{
    public class ContinueGameControl : MonoBehaviour
    {    
        [SerializeField] public Transform gamesContainer;
        [SerializeField] public GameObject gameTilePrefab;

        private async void Awake()
        {
            foreach (Transform child in gamesContainer)
            {
                Destroy(child.gameObject);
            }

            GamesToContinueDTO gamesToContinue = await DialogueEngineManager.Instance.GetGamesToContinueAsync();         
           
            var sortedGames = gamesToContinue.GamesToContinue.OrderByDescending(g => g.LastSaveDate).ToList();

            sortedGames = sortedGames.Take(4).ToList();  

            sortedGames = sortedGames.Take(4).ToList();

            foreach (CreatedGameDTO game in sortedGames)
            {
                GameObject tile = Instantiate(gameTilePrefab, gamesContainer);

                tile.transform.Find("Title").GetComponent<TMP_Text>().text = $"Tytuł: {game.Title}";
                tile.transform.Find("CurrentSceneNumber").GetComponent<TMP_Text>().text = $"Scena: {game.CurrentSceneNumber}/{game.MaxSceneNumber}";
                DateTime displayDate = DateTime.Parse(game.LastSaveDate);
                string data = displayDate.Year + "." + displayDate.Month + "." + displayDate.Day;
                string godzina = displayDate.Hour + "." + displayDate.Minute;
                tile.transform.Find("LastSaveDate").GetComponent<TMP_Text>().text = $"Ostatni zapis: {data} godz. {godzina}";
                Transform deleteBtnTransform = tile.transform.Find("X");

                if (deleteBtnTransform != null)
                {
                    Button deleteBtn = deleteBtnTransform.GetComponent<Button>();
                    // Dodajemy listener. Przekazujemy 'game' (dane) i 'tile' (obiekt wizualny), żeby go usunąć
                    deleteBtn.onClick.AddListener(() => OnDeleteGameClicked(game, tile));
                }
                else
                {
                    Debug.LogWarning("Nie znaleziono przycisku 'DeleteButton' w prefabie!");
                }
                Button btn = tile.GetComponent<Button>();
                btn.onClick.AddListener(() => OnGameTileClicked(game));
            }
        }

        private async Task OnGameTileClicked(CreatedGameDTO game)
        {
            string scenario = game.Title;
            int sceneCurrent = game.CurrentSceneNumber;
            int sceneMax = game.MaxSceneNumber;

            string[] parameters = { scenario, sceneCurrent.ToString()};

            CurrentSaveManager.Instance.SetCurrentSave(game);
            CurrentSaveManager.Instance.SetIsNewGame(false);
            Debug.Log("IsNewGame ustawione na false");

            SceneScriptDTO scene = await DialogueEngineManager.Instance.GetSceneAsync(parameters);
            if (scene != null) 
            {
                DialogueContextManager.SetContext(game.GameHistory);
                GameSession.StartSession(scenario, sceneCurrent);
                SceneManager.LoadScene("NewGame");

                Debug.Log($"Game Loaded. Scene {GameSession.CurrentSceneNumber}");
            }
            else Debug.Log("Error loading game");
        }
        private async void OnDeleteGameClicked(CreatedGameDTO game, GameObject tileObj)
        {
            // Szukamy przycisku usuwania, żeby go zablokować na czas operacji (opcjonalne)
            Button btn = tileObj.transform.Find("X")?.GetComponent<Button>();
            if (btn != null) btn.interactable = false;

            // --- NAPRAWA: Wywołujemy faktyczną funkcję z DialogueEngineManager ---
            // Przekazujemy cały obiekt 'game', tak jak przygotowałeś w managerze
            bool success = await DialogueEngineManager.Instance.DeleteGameAsync(game);

            if (success)
            {
                // Jeśli backend potwierdził usunięcie (zwrócił true), usuwamy kafelek
                Destroy(tileObj);
                Debug.Log($"Pomyślnie usunięto zapis gry: {game.Title}");
            }
            else
            {
                // Jeśli coś poszło nie tak, logujemy błąd i nie usuwamy kafelka
                Debug.LogError("Błąd podczas usuwania zapisu z pliku JSON.");

                // Odblokowujemy przycisk, żeby można było spróbować ponownie
                if (btn != null) btn.interactable = true;
            }
        }
    }
}