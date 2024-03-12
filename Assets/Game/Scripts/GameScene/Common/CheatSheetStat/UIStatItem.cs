using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Game.Scripts.GameScene.Common.CheatSheetStat
{
    public class UIStatItem : MonoBehaviour
    {
        [SerializeField] private Image m_StatIcon;
        [SerializeField] private Text m_StatName;
        [SerializeField] private Text m_StatValue;

        public void Setup(string statKey, Engine.Stat stat)
        {
            m_StatName.text = statKey;
            m_StatValue.text = stat.Value.TruncateValue();
        }
    }
}
