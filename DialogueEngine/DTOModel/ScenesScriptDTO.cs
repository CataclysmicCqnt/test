using System;
using System.Collections.Generic;
using System.Text;

namespace DTOModel
{
    /// <summary>
    /// DTO reprezentuj¹cy zestaw scen i zakoñczeñ w grze.
    /// </summary>
    [Serializable]
    public class ScenesScriptDTO
    {
        /// <summary>
        /// Tablica scen w grze.
        /// </summary>
        public SceneScriptDTO[] Scenes;

        /// <summary>
        /// Tablica mo¿liwych zakoñczeñ gry.
        /// </summary>
        public EndingDTO[] Endings;
    }
}
