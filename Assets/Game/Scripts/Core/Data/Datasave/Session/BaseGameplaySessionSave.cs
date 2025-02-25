using Assets.Game.Scripts.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gameplay.Data
{
    [System.Serializable]
    public class BaseGameplaySessionSave : BaseDatasave
    {
        public EGameMode GameMode = EGameMode.Dungeon;
        public float HpPercentage;
        public ECharacter Character;
        // List Buff Dungeon

        public BaseGameplaySessionSave() { }

        public BaseGameplaySessionSave(string key) : base(key)
        {
        }

        public override void Fix()
        {
        }
    }
}
