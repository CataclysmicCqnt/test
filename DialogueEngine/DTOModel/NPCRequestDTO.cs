using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace DTOModel
{
    [Serializable]
    public class NPCRequestDTO
    {
        [JsonProperty("sceneDescription")]
        public string SceneDescription;
        [JsonProperty("userText")]
        public string UserText;
        [JsonProperty("npcName")]
        public string NPCName;
    }
}
