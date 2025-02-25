using Engine;
using Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gameplay
{
    [System.Serializable]
    public class OutfitData
    {
        public int OutfitId { get; }
        public EquipmentHandler EquipmentHandler { get; }

        public OutfitData(int id, IStatGroup stat) 
        {
            OutfitId = id;
            EquipmentHandler = new EquipmentHandler(stat);
        }
    }
}
