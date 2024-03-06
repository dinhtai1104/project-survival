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
        private Dictionary<int, OutfitData> m_OutfitDatas;


        public IStatGroup PlayerStats => m_PlayerStats;
        public Dictionary<ECharacter, CharacterDataInfo> StatsHolder => m_StatsHolder;
        public ECharacter CurrentCharacter { get => m_CurrentCharacter; set => m_CurrentCharacter = value; }
        public Dictionary<int, OutfitData> OutfitDatas => m_OutfitDatas;

        public PlayerData(ECharacter currentCharacter = ECharacter.Normal)
        {
            this.m_CurrentCharacter = currentCharacter;
            m_StatsHolder = new Dictionary<ECharacter, CharacterDataInfo>();
            m_OutfitDatas = new Dictionary<int, OutfitData>();

            foreach (var character in CharacterExtension.AllCharacter)
            {
                var data = new CharacterDataInfo(character);
                m_StatsHolder.Add(character, data);
            }

            m_OutfitDatas.Add(0, new OutfitData(0, m_PlayerStats));
            m_PlayerStats = m_StatsHolder[m_CurrentCharacter].Stats;
        }
    }
}
