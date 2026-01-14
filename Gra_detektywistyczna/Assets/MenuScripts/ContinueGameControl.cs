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

            foreach (CreatedGameDTO game in sortedGames)
            {
                GameObject tile = Instantiate(gameTilePrefab, gamesContainer);

                tile.transform.Find("Title").GetComponent<TMP_Text>().text = $"Tytuł: {game.Title}";
                tile.transform.Find("CurrentSceneNumber").GetComponent<TMP_Text>().text = $"Scena: {game.CurrentSceneNumber}/{game.MaxSceneNumber}";
                DateTime displayDate = DateTime.Parse(game.LastSaveDate);
                string data = displayDate.Year + "." + displayDate.Month + "." + displayDate.Day;
                string godzina = displayDate.Hour + "." + displayDate.Minute;
                tile.transform.Find("LastSaveDate").GetComponent<TMP_Text>().text = $"Ostatni zapis: {data} godz. {godzina}";

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
                GameSession.StartSession(scenario, sceneCurrent, sceneMax);
                SceneManager.LoadScene("NewGame");

                Debug.Log($"Game Loaded. Scene {GameSession.CurrentSceneNumber}");
            }
            else Debug.Log("Error loading game");
        }
    }
}