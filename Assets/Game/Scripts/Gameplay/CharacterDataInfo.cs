using Assets.Game.Scripts.Enums;
using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gameplay
{
    [System.Serializable]
    public class CharacterDataInfo
    {
        public ECharacter Character;
        public IStatGroup Stats;

        public CharacterDataInfo(ECharacter character)
        {
            Character = character;
            Stats = PlayerStat.Default();
        }



    }
}
