using DTOModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DialogueEngine
{
    public class DialogueEngine
    {
        private string _databasePath = AppDomain.CurrentDomain.BaseDirectory + "../../../../../Gra_detektywistyczna/Assets/Database";

        public string GetGamesToContinue(string[] parameters)
        {
            string games = File.ReadAllText(_databasePath + "/SavedGames.json");

            GamesToContinueDTO gameDTOs = JsonConvert.DeserializeObject<GamesToContinueDTO>(games);

            return JsonConvert.SerializeObject(gameDTOs);
        }

        public string GetSettings(string[] parameters)
        {
            string options = File.ReadAllText(_databasePath + "/Settings.json");
            return options;
        }

        public string SaveSettings(string[] parameters)
        {
            if (parameters.Length == 0) return string.Empty;

            File.WriteAllText(_databasePath + "/Settings.json", parameters[0]);

            return string.Empty;
        }
    }
}
