using BansheeGz.BGDatabase;
using Engine;
using ExtensionKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Game.Scripts.Core.Data.Database.LevelUpBuff
{
    [System.Serializable]
    public class LevelUpBuffEntity : IWeightable
    {
        public int Id;
        public List<ModifierData> Modifiers;
        public float Ratio;
        public float Weight => Ratio;

        public LevelUpBuffEntity() 
        { 
            Modifiers = new List<ModifierData>();
        }
        public LevelUpBuffEntity(BGEntity e) : this()
        {
            Id = e.Get<int>("Id");
            Enum.TryParse(e.Get<string>("ModType"), out EStatMod modType);
            var statName = e.Get<string>("Stat");
            Ratio = e.Get<float>("Ratio");
            var listMod = e.Get<List<float>>("Modifier");
            if (listMod.IsNotNull())
            {
                foreach (var modi in listMod)
                {
                    var modifierData = new ModifierData(statName, new StatModifier(modType, modi));
                    Modifiers.Add(modifierData);
                }
            }
        }
    }
}
