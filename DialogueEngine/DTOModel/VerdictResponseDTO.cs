using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace DTOModel
{
    [Serializable]
    public class VerdictResponseDTO
    {
        [JsonProperty("speech")]
        public string speech;
        [JsonProperty("isPlayerRight")]
        public bool isPlayerRight;
    }
}
