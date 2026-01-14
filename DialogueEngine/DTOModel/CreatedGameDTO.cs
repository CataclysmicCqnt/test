using System;
using Newtonsoft.Json;

namespace DTOModel
{
    /// <summary>
    /// DTO reprezentujący utworzoną grę.
    /// </summary>
    /// <remarks>
    /// Przechowuje podstawowe informacje o stanie gry, numerze sceny, historii oraz dacie ostatniego zapisu.
    /// </remarks>
    [Serializable]
    public class CreatedGameDTO
    {
        /// <summary>
        /// Tytuł gry.
        /// </summary>
        public string Title;

        /// <summary>
        /// Numer bieżącej sceny w grze.
        /// </summary>
        public int CurrentSceneNumber;

        /// <summary>
        /// Maksymalny numer sceny w grze.
        /// </summary>
        public int MaxSceneNumber;

        /// <summary>
        /// Historia gry w formacie JSON.
        /// </summary>
        public string GameHistory;

        /// <summary>
        /// Data ostatniego zapisu gry.
        /// </summary>
        public string LastSaveDate;
    }
}
