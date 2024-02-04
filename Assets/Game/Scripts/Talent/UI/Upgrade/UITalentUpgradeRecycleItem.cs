using Assets.Game.Scripts.Utilities;
using Mosframe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Game.Scripts.Talent.UI.Upgrade
{
    public class UITalentUpgradeRecycleItem : MonoBehaviour, IDynamicScrollViewItem
    {
        [SerializeField] private UITalentUpgradeViewItem container;
        [SerializeField] private Image talentQualityImage;
        [SerializeField] private Image talentImage;
        private int index = -1;
        public int getIndex()
        {
            return index;
        }

        public void onUpdateItem(int index)
        {
            this.index = index;
            if (index == -1) return;
            UpdateUI();
        }

        private void UpdateUI()
        {
            var entity = container.GetEntity(index);
            talentQualityImage.sprite = ResourcesLoader.Instance.GetSprite(AtlasName.EquipmentRarity, entity.Quality.ToString());
            talentImage.sprite = ResourcesLoader.Instance.GetSprite(AtlasName.Talent, entity.StatKey.ToString());
        }
    }
}
