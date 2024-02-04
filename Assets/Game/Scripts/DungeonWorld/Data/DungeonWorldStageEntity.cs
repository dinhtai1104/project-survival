using BansheeGz.BGDatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Game.Scripts.DungeonWorld.Data
{
    [System.Serializable]
    public class DungeonWorldStageEntity
    {
        public int Index;
        public int Dungeon;
        public int Stage;
        public List<LootParams> Reward;

        public DungeonWorldStageEntity(BGEntity e)
        {
            Dungeon = e.Get<int>("Dungeon");
            Index = e.Get<int>("Index");
            Stage = e.Get<int>("Stage");

            Reward = new List<LootParams>();
            var data = e.Get<List<string>>("Reward");
            foreach (var model in data)
            {
                Reward.Add(new LootParams(model));
            }
        }
    }
}
