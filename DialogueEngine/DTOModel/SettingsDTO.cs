using System;
using System.Collections.Generic;
using System.Text;

namespace DTOModel
{
    /// <summary>
    /// DTO reprezentujący ustawienia gry.
    /// </summary>
    [Serializable]
    public class SettingsDTO
    {
        /// <summary>
        /// Poziom głośności w grze.
        /// </summary>
        public int Volume;
    }
}
