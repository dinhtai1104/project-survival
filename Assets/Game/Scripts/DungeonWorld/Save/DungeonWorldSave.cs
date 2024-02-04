using Assets.Game.Scripts.DungeonWorld.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Game.Scripts.DungeonWorld.Save
{
    [System.Serializable]
    public class DungeonWorldSave
    {
        public List<DungeonWorldStageSave> Stages = new List<DungeonWorldStageSave>();

        public DungeonWorldSave(DungeonWorldEntity model)
        {
            foreach (var md in model.Stages)
            {
                Stages.Add(new DungeonWorldStageSave(md.Dungeon, md.Stage, false));
            }
        }

        public void Claim(int Stage)
        {
            var find = Stages.Find(t => t.Stage == Stage);
            if (find == null) return;
            find.IsClaimed = true;
            Save();
        }

        public void Save()
        {
            DataManager.Save.DungeonWorld.Save();
        }

        public bool IsClaim(int Stage)
        {
            var find = Stages.Find(t => t.Stage == Stage);
            if (find == null) return false;
            return find.IsClaimed;
        }

        public bool Fix(DungeonWorldEntity model)
        {
            bool save = false;
            for (int i = 0; i < model.Stages.Count; i++)
            {
                bool fix = Stages[i].Fix(model.Stages[i]);
                if (fix)
                {
                    save = true;
                }
            }
            return save;
        }
    }
}
