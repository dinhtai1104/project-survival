using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Game.Scripts.GameScene.Common
{
    public class UIStatModifierItem : MonoBehaviour
    {
        [SerializeField] private Text m_ModifierTxt;
        [SerializeField] private Text m_AffixModifierTxt;
        
        public void SetMainModifier(string modifierText)
        {
            m_ModifierTxt.text = modifierText;
            
            // Auto set affix to null for set manually
            m_AffixModifierTxt.text = string.Empty;
        }

        public void SetAffixModifierTxt(string modifierText)
        {
            m_AffixModifierTxt.text = modifierText;
        }
    }
}
