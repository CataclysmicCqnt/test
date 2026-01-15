using System;
using System.Collections.Generic;
using System.Text;

namespace DTOModel
{
    /// <summary>
    /// DTO reprezentujący NPC (postać niezależną w grze).
    /// </summary>
    /// <remarks>
    /// Zawiera podstawowe informacje o NPC, takie jak jego imię, rola, opis i portret.
    /// </remarks>
    [Serializable]
    public class NPCDTO
    {
        /// <summary>
        /// Imię lub nazwa NPC.
        /// </summary>
        public string name;

        /// <summary>
        /// Rola NPC w grze lub w historii.
        /// </summary>
        public string role;

        /// <summary>
        /// Opis NPC.
        /// </summary>
        public string description;

        /// <summary>
        /// Identyfikator wizerunku postaci.
        /// </summary>
        public string protrait;
    }
}
