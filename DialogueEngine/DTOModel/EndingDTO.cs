using System;
using System.Collections.Generic;
using System.Text;

namespace DTOModel
{
    [Serializable]
    public class EndingDTO
    {
        public string Name;
        public string Description;

        public NPCDTO[] Npcs;
        public ItemDTO[] Items;



    }

}