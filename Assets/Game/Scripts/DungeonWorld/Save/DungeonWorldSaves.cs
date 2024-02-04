using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Game.Scripts.DungeonWorld.Save
{
    [System.Serializable]
    public class DungeonWorldSaves : BaseDatasave
    {
        [ShowInInspector]
        public Dictionary<int, DungeonWorldSave> Saves = new Dictionary<int, DungeonWorldSave>();

        public DungeonWorldSaves(string key) : base(key)
        {
            Saves = new Dictionary<int, DungeonWorldSave>();
            var db = DataManager.Base.DungeonWorld;

            foreach (var model in db.Dictionary.Values.ToList())
            {
                if (!Saves.ContainsKey(model.Dungeon))
                {
                    Saves.Add(model.Dungeon, new DungeonWorldSave(model));
                }
            }
        }

        public override void Fix()
        {
            var db = DataManager.Base.DungeonWorld;
            bool save = false;
            foreach (var model in db.Dictionary.Values.ToList())
            {
                if (!Saves.ContainsKey(model.Dungeon))
                {
                    Saves.Add(model.Dungeon, new DungeonWorldSave(model));
                }
                bool fix = Saves[model.Dungeon].Fix(model);
                if (fix)
                {
                    save = true;
                }
            }
            if (save)
            {
                Save();
            }
        }

        public void ClaimReward(int Dungeon, int Stage)
        {
            Saves[Dungeon].Claim(Stage);
        }
        public bool IsClaimedReward(int Dungeon, int Stage)
        {
            return Saves[Dungeon].IsClaim(Stage);
        }

        public DungeonWorldStageSave Get(int dungeon, int stage)
        {
            return Saves[dungeon].Stages.Find(t => t.Stage == stage);
        }

        public bool IsClaimAll(int dungeon)
        {
            foreach (var st in Saves[dungeon].Stages)
            {
                if (st.IsClaimed == false) return false;
            }
            return true;
        }
    }
}
