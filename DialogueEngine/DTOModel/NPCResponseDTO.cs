using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace DTOModel
{
    /// <summary>
    /// DTO reprezentujący odpowiedź NPC na pytanie gracza
    /// </summary>
    [Serializable]
    public class NPCResponseDTO
    {
        /// <summary>
        /// Tekstowa wypowiedź NPC.
        /// </summary>
        public string Speech;

        /// <summary>
        /// Działanie, które NPC wykonuje w odpowiedzi.
        /// </summary>
        public string Action;

        /// <summary>
        /// Zamiar NPC w odpowiedzi.
        /// </summary>
        public string Intent;
    }
}
