using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace DTOModel
{
    [Serializable]
    public class NPCResponseDTO
    {
        public string Speech;
        public string Action;
        public string Intent;
    }
}
