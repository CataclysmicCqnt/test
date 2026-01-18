using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace DTOModel
{
    [Serializable]
    public class VerdictRequestDTO
    {
        [JsonProperty("accusedName")]
        public string AccusedName;
        [JsonProperty("endings")]
        public EndingDTO[] Ending;
    }
}
