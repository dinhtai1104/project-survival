using BansheeGz.BGDatabase;
using Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Game.Scripts.Core.Data.Database.Rarity
{
    [System.Serializable]
    public class RarityEntity
    {
        public ERarity Rarity;
        public int MaxLevel;

        public RarityEntity(BGEntity e)
        {
            Enum.TryParse(e.Get<string>("Rarity"), out Rarity);
            MaxLevel = e.Get<int>("MaxLevel");
        }
    }
}
