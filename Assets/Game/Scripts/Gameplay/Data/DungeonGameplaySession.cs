using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gameplay.Data
{
    [System.Serializable]
    public class DungeonGameplaySession : BaseGameplaySession
    {
        private int m_CurrentDungeon;
        private int m_CurrentWave;

        public int CurrentDungeon => m_CurrentDungeon;
        public int CurrentWave => m_CurrentWave;
    }
}
