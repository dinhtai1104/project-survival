using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Game.Scripts.Core.Data.Datasave.Dungeon
{
    [System.Serializable]
    public class DungeonSave : BaseDatasave
    {
        public List<int> DungeonCleared = new List<int>();

        public int DungeonCurrent;
        public int WaveCurrent;

        public DungeonSave(string key) : base(key)
        {
        }

        public DungeonSave()
        {
        }

        public override void Fix()
        {
        }

        public void ClearDungeon(int dungeonId)
        {
            DungeonCleared.Add(dungeonId);
            DungeonCurrent++;
            Save();
        }
        public bool CanPlayDungeon(int dungeonId)
        {
            return DungeonCleared.Contains(dungeonId) || DungeonCurrent >= dungeonId;
        }

        public void SetBestWave(int waveId)
        {
            this.WaveCurrent = waveId;
            Save();
        }
    }
}
