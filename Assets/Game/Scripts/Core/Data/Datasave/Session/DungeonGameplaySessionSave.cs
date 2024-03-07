using Assets.Game.Scripts.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gameplay.Data
{
    [System.Serializable]
    public class DungeonGameplaySessionSave : BaseGameplaySessionSave
    {
        public int CurrentDungeon;
        public int CurrentWave;
        public EBattleResult Result;
        public DungeonGameplaySessionSave()
        {
        }

        public DungeonGameplaySessionSave(string key) : base(key)
        {
        }

    }
}
