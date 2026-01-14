using System;
using System.Collections.Generic;
using System.Text;

namespace DTOModel
{
    /// <summary>
    /// DTO reprezentuj¹cy pe³n¹ scenê w grze, w tym t³o, postacie i przedmioty.
    /// </summary>
    [Serializable]
    public class SceneScriptDTO
    {
        /// <summary>
        /// Nazwa sceny.
        /// </summary>
        public string Name;

        /// <summary>
        /// Opis sceny.
        /// </summary>
        public string Description;

        /// <summary>
        /// Identyfiakator t³a sceny.
        /// </summary>
        public string Background;

        /// <summary>
        /// Tablica NPC obecnych w scenie.
        /// </summary>
        public NPCDTO[] Npcs;

        /// <summary>
        /// Tablica przedmiotów obecnych w scenie.
        /// </summary>
        public ItemDTO[] Items;
    }
}
