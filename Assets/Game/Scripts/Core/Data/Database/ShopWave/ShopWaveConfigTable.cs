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
    public class ShopWaveConfigTable : DataTable<EShopWaveItem, Dictionary<ERarity, ShopWaveConfigEntity>>
    {
        public override void GetDatabase()
        {
            DB_ShopWaveConfig.ForEachEntity(e => Get(e));
        }
        
        private void Get(BGEntity e)
        {
            var config = new ShopWaveConfigEntity(e);
            if (!Dictionary.ContainsKey(config.Type))
            {
                Dictionary.Add(config.Type, new Dictionary<ERarity, ShopWaveConfigEntity>());
            }
            if (!Dictionary[config.Type].ContainsKey(config.Rarity))
            {
                Dictionary[config.Type].Add(config.Rarity, config);
            }
        }
    }
}
