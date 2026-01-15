using System;
using System.Collections.Generic;
using System.Text;

namespace DTOModel
{
    /// <summary>
    /// DTO reprezentujący pojedynczy przedmiot w grze.
    /// </summary>
    /// <remarks>
    /// Zawiera podstawowe informacje o przedmiocie, takie jak jego nazwa, opis i dodatkowe wskazówki dla gracza.
    /// </remarks>
    [Serializable]
    public class ItemDTO
    {
        /// <summary>
        /// Nazwa przedmiotu.
        /// </summary>
        public string name;

        /// <summary>
        /// Opis przedmiotu.
        /// </summary>
        public string description;

        /// <summary>
        /// Dodatkowe wskazówki lub podpowiedzi związane z przedmiotem.
        /// </summary>
        public string hints;
    }
}
