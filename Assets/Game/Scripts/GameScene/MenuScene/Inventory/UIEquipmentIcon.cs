using Framework;
using UnityEngine;

namespace Assets.Game.Scripts.GameScene.MenuScene.Inventory
{
    public class UIEquipmentIcon : UIGeneralIcon
    {
        private EquipableItem m_Item;

        public EquipableItem Item => m_Item;
        public void SetEquipableItem(EquipableItem item)
        {
            m_Item = item;
            SetInfo();
        }

        private void SetInfo()
        {
            SetValueAmount(0);
            SetSpriteIcon(null);
            SetSpriteFrame(null);
        }
    }
}
