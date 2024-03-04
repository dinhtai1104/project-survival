using Assets.Game.Scripts.Enums;
using Engine;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gameplay
{
    [System.Serializable]
    public class PlayerData
    {
        private ECharacter m_CurrentCharacter = ECharacter.Normal;
        private IStatGroup m_PlayerStats;
        private Dictionary<ECharacter, CharacterDataInfo> m_StatsHolder;

        public IStatGroup PlayerStats { get => m_PlayerStats; set => m_PlayerStats = value; }
        public Dictionary<ECharacter, CharacterDataInfo> StatsHolder { get => m_StatsHolder; set => m_StatsHolder = value; }
        public ECharacter CurrentCharacter { get => m_CurrentCharacter; set => m_CurrentCharacter = value; }

        public PlayerData(ECharacter currentCharacter = ECharacter.Normal)
        {
            this.m_CurrentCharacter = currentCharacter;

            foreach (var character in CharacterExtension.AllCharacter)
            {
                var data = new CharacterDataInfo(character);
                m_StatsHolder.Add(character, data);
            }

            m_PlayerStats = m_StatsHolder[m_CurrentCharacter].Stats;
        }
    }
}
