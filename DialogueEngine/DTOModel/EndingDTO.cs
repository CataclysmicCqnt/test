using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace DTOModel
{
    /// <summary>
    /// DTO reprezentujący zakończenie gry.
    /// </summary>
    [Serializable]
    public class EndingDTO
    {
        /// <summary>
        /// Nazwa oskarżonego.
        /// </summary>
        [JsonProperty("accusedName")]
        public string AccusedName;

        /// <summary>
        /// Opis zakończenia.
        /// </summary>
        [JsonProperty("description")]
        public string Description;

        /// <summary>
        /// Czy oskarżony jest winny morderstwa
        /// </summary>
        [JsonProperty("isMurderer")]
        public bool IsMurderer;


    }
}
