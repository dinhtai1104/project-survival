using Assets.Game.Scripts.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gameplay.Data
{
    [System.Serializable]
    public class BaseGameplaySession
    {
        public EGameMode GameMode = EGameMode.Dungeon;
    }
}
