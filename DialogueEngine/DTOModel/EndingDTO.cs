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
        /// Nazwa zakończenia.
        /// </summary>
        public string Name;

        /// <summary>
        /// Opis zakończenia.
        /// </summary>
        public string Description;

        /// <summary>
        /// Tablica obiektów <see cref="NPCDTO"/> powiązanych z zakończeniem.
        /// </summary>
        public NPCDTO[] Npcs;

        /// <summary>
        /// Tablica obiektów <see cref="ItemDTO"/> powiązanych z zakończeniem.
        /// </summary>
        public ItemDTO[] Items;
    }
}
