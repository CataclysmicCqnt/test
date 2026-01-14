using System;
using System.Collections.Generic;
using System.Text;

namespace DTOModel
{
    /// <summary>
    /// DTO reprezentujący listę gier, które można kontynuować.
    /// </summary>
    [Serializable]
    public class GamesToContinueDTO
    {
        /// <summary>
        /// Tablica obiektów <see cref="CreatedGameDTO"/> z grami do kontynuowania.
        /// </summary>
        public CreatedGameDTO[] GamesToContinue;
    }
}
