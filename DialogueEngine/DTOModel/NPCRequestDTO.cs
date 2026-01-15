using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace DTOModel
{
    /// <summary>
    /// DTO reprezentujący zapytanie do NPC w grze.
    /// </summary>
    /// <remarks>
    /// Zawiera informacje o scenie, tekście użytkownika oraz nazwie NPC, do którego kierowane jest zapytanie.
    /// Służy do przesłania danych z klienta do backendu AI.
    /// </remarks>
    [Serializable]
    public class NPCRequestDTO
    {
        /// <summary>
        /// Opis sceny, w której znajduje się NPC.
        /// </summary>
        [JsonProperty("sceneDescription")]
        public string SceneDescription;

        /// <summary>
        /// Tekst wpisany przez użytkownika.
        /// </summary>
        [JsonProperty("userText")]
        public string UserText;

        /// <summary>
        /// Nazwa NPC, do którego kierowane jest zapytanie.
        /// </summary>
        [JsonProperty("npcName")]
        public string NPCName;
    }
}
