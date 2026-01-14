using System;
using System.Collections.Generic;
using System.Text;

namespace DTOModel
{
    /// <summary>
    /// DTO reprezentujący scenę w grze.
    /// </summary>
    [Serializable]
    public class SceneDTO
    {
        /// <summary>
        /// Nazwa lokalizacji sceny.
        /// </summary>
        public string LocationName;

        /// <summary>
        /// Opis sceny lub prompt kierujący dalszym przebiegiem gry.
        /// </summary>
        public string ScenePrompt;
    }
}
