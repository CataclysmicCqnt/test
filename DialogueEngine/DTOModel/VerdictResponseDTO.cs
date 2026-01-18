using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace DTOModel
{
    [Serializable]
    public class VerdictResponseDTO
    {
        public string Speech;
        public bool IsPlayerRight;
    }
}
