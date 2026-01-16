using AIClient;
using DTOModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DialogueEngine
{
    /// <summary>
    /// Silnik dialogowy odpowiedzialny za obsługę gry, komunikację z AI oraz zarządzanie zapisanymi grami i ustawieniami.
    /// </summary>
    public class DialogueEngine
    {
        private readonly string _databasePath = AppDomain.CurrentDomain.BaseDirectory + "../../../../../Gra_detektywistyczna/Assets/Database";
        private readonly AICommunication _aICommunication = new AICommunication();

        /// <summary>
        /// Pobiera listę gier, które można kontynuować.
        /// </summary>
        /// <param name="parameters">Nie używane w tej metodzie.</param>
        /// <returns>JSON zawierający listę zapisanych gier.</returns>
        public string GetGamesToContinue(string[] parameters)
        {
            string games = File.ReadAllText(_databasePath + "/SavedGames.json");

            if (games == null || games == "")
                return JsonConvert.SerializeObject(new GamesToContinueDTO());

            GamesToContinueDTO gameDTOs = JsonConvert.DeserializeObject<GamesToContinueDTO>(games);

            var settings = new JsonSerializerSettings
            {
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                DateTimeZoneHandling = DateTimeZoneHandling.Utc
            };
            return JsonConvert.SerializeObject(gameDTOs, settings);
        }

        /// <summary>
        /// Pobiera aktualne ustawienia gry.
        /// </summary>
        /// <param name="parameters">Nie używane w tej metodzie.</param>
        /// <returns>JSON z ustawieniami gry.</returns>
        public string GetSettings(string[] parameters)
        {
            string options = File.ReadAllText(_databasePath + "/Settings.json");
            return options;
        }

        /// <summary>
        /// Zapisuje ustawienia gry.
        /// </summary>
        /// <param name="parameters">Tablica, w której pierwszy element zawiera JSON z ustawieniami.</param>
        /// <returns>Pusty string w przypadku sukcesu.</returns>
        public string SaveSettings(string[] parameters)
        {
            if (parameters.Length == 0) return string.Empty;

            File.WriteAllText(_databasePath + "/Settings.json", parameters[0]);

            return string.Empty;
        }

        /// <summary>
        /// Wysyła zapytanie do NPC i zwraca jego odpowiedź z backendu AI.
        /// </summary>
        /// <param name="parameters">Tablica, w której pierwszy element zawiera JSON z NPCRequestDTO.</param>
        /// <returns>Odpowiedź NPC w formacie JSON.</returns>
        public async Task<string> AskNPC(string[] parameters)
        {
            if (parameters.Length == 0) return string.Empty;

            NPCRequestDTO nPCRequestDTO = JsonConvert.DeserializeObject<NPCRequestDTO>(parameters[0]);

            return await _aICommunication.GenerateNPCResponseAsync(nPCRequestDTO);
        }

        /// <summary>
        /// Generuje nową scenę przy użyciu backendu AI.
        /// </summary>
        /// <param name="parameters">Tablica, w której pierwszy element zawiera JSON z SceneDTO.</param>
        /// <returns>JSON z wygenerowaną sceną.</returns>
        public async Task<string> GenerateNewScene(string[] parameters)
        {
            if (parameters.Length == 0) return string.Empty;

            SceneDTO sceneDTO = JsonConvert.DeserializeObject<SceneDTO>(parameters[0]);

            return await _aICommunication.GenerateNewSceneAsync(sceneDTO);
        }

        /// <summary>
        /// Pobiera konkretną scenę z zapisanych plików scenariusza.
        /// </summary>
        /// <param name="parameters">Tablica, gdzie pierwszy element to nazwa pliku scenariusza, a drugi numer sceny.</param>
        /// <returns>JSON reprezentujący wybraną scenę lub null, jeśli nie istnieje.</returns>
        public string GetScene(string[] parameters)
        {
            int sceneNumber = Convert.ToInt32(parameters[1]);

            string scenes = File.ReadAllText(_databasePath + "/" + parameters[0] + ".json");
            if (scenes == null || scenes == "") return null;

            ScenesScriptDTO scenesDTO = JsonConvert.DeserializeObject<ScenesScriptDTO>(scenes);

            if (scenesDTO == null) return null;
            if (sceneNumber > scenesDTO.Scenes.Length || sceneNumber <= 0) return null;

            var settings = new JsonSerializerSettings
            {
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                DateTimeZoneHandling = DateTimeZoneHandling.Utc
            };

            return JsonConvert.SerializeObject(scenesDTO.Scenes[sceneNumber - 1], settings);
        }

        /// <summary>
        /// Zapisuje stan gry do pliku zapisów.
        /// </summary>
        /// <param name="parameters">Tablica, w której pierwszy element zawiera JSON z CreatedGameDTO.</param>
        /// <returns>Komunikat o sukcesie lub błędzie.</returns>
        public string SaveGame(string[] parameters)
        {
            try
            {
                if (parameters.Length == 0) return "Error: No parameters";

                CreatedGameDTO newGame = JsonConvert.DeserializeObject<CreatedGameDTO>(parameters[0]);

                string gamesJsonPath = _databasePath + "/SavedGames.json";
                List<CreatedGameDTO> gamesList = new List<CreatedGameDTO>();

                if (File.Exists(gamesJsonPath))
                {
                    string gamesJson = File.ReadAllText(gamesJsonPath);
                    if (!string.IsNullOrEmpty(gamesJson))
                    {
                        var existingGames = JsonConvert.DeserializeObject<GamesToContinueDTO>(gamesJson);
                        if (existingGames != null && existingGames.GamesToContinue != null)
                        {
                            gamesList = existingGames.GamesToContinue.ToList();
                        }
                    }
                }

                gamesList.Add(newGame);

                GamesToContinueDTO allGames = new GamesToContinueDTO
                {
                    GamesToContinue = gamesList.ToArray()
                };

                string updatedJson = JsonConvert.SerializeObject(allGames, Formatting.Indented);
                File.WriteAllText(gamesJsonPath, updatedJson);

                return "Game saved successfully";
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }
        /// <summary>
        /// Losuje scenariusz z dostępnych plików scenariusza.
        /// </summary>
        /// <param name="parameters">Nie używane w tej metodzie.</param>
        /// <returns>Nazwę wylosowanego scenariusza.</returns>
        public string GetRandomScenario(string[] parameters)
        {
            try
            {
                string databasePath = _databasePath;

                if (!Directory.Exists(databasePath))
                {
                    Console.WriteLine($"Database path does not exist: {databasePath}");
                    return "scenerio_apollo"; // Domyślny scenariusz
                }

                var scenarioFiles = Directory.GetFiles(databasePath, "*.json")
                    .Select(Path.GetFileNameWithoutExtension)
                    .Where(name => !name.Equals("Settings", StringComparison.OrdinalIgnoreCase)
                                && !name.Equals("SavedGames", StringComparison.OrdinalIgnoreCase))
                    .ToArray();

                Console.WriteLine($"Found {scenarioFiles.Length} scenario files: {string.Join(", ", scenarioFiles)}");

                if (scenarioFiles.Length == 0)
                {
                    Console.WriteLine("No scenario files found, using default");
                    return "scenerio_apollo";
                }

                Random random = new Random();
                int randomIndex = random.Next(0, scenarioFiles.Length);
                string selectedScenario = scenarioFiles[randomIndex];

                Console.WriteLine($"Randomly selected scenario: {selectedScenario}");

                string scenarioPath = Path.Combine(databasePath, selectedScenario + ".json");
                string jsonContent = File.ReadAllText(scenarioPath);
                var scenesDTO = JsonConvert.DeserializeObject<ScenesScriptDTO>(jsonContent);

                if (scenesDTO == null || scenesDTO.Scenes == null || scenesDTO.Scenes.Length == 0)
                {
                    Console.WriteLine($"Scenario {selectedScenario} is empty or invalid, using default");
                    return "scenerio_apollo";
                }

                return selectedScenario;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting random scenario: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
                return "scenerio_apollo";
            }
        }
    }
}
