using Assets.Game.Scripts.Talent.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Assets.Game.Scripts.Talent.UI.Upgrade
{
    public class UITalentUpgradeInfor : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI levelCurrentTxt;
        [SerializeField] private TextMeshProUGUI nameTalentTxt;
        [SerializeField] private TextMeshProUGUI statTalentTxt;

        public void Active(bool active)
        {
            gameObject.SetActive(active);
        }

        public void Set(TalentEntity entity, int level)
        {
            Active(true);
            levelCurrentTxt.text = level.ToString();
            nameTalentTxt.text = I2Localize.GetLocalize("Stat/" + entity.StatKey);
            statTalentTxt.text = entity.GetAttribute(1).Modifier.ToString(entity.Format);
        }
    }
}
