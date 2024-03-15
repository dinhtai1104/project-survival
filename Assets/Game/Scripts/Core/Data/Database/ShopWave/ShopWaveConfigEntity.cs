using Assets.Game.Scripts.GameScene.ShopWave;
using BansheeGz.BGDatabase;
using Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Game.Scripts.Core.Data.Database.ShopWave
{
    [System.Serializable]
    public class ShopWaveConfigEntity
    {
        public EShopWaveItem Type;
        public ERarity Rarity;
        public int MinWaveUnlock;
        public float ChancePerWave;
        public float MaxChance;

        public ShopWaveConfigEntity(BGEntity e)
        {
            Enum.TryParse(e.Get<string>("Type"), out Type);
            Enum.TryParse(e.Get<string>("Rarity"), out Rarity);
            MinWaveUnlock = e.Get<int>("MinWaveUnlock");
            ChancePerWave = e.Get<float>("ChancePerWave");
            MaxChance = e.Get<float>("MaxChance");
        }
    }
}
