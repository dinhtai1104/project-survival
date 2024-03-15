using Assets.Game.Scripts.Core.Data.Database.Buff;
using Assets.Game.Scripts.Core.Data.Database.Equipment;
using Assets.Game.Scripts.Core.Data.Database.Equipment.Weapon;
using ExtensionKit;
using Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Game.Scripts.GameScene.ShopWave
{
    [System.Serializable]
    public class ShopWaveItemModel
    {
        private EShopWaveItem m_TypeItem;
        private ERarity m_Rarity;
        private string m_Id;
        private int m_Price;
        public int Price
        {
            get => m_Price;
            set => m_Price = value;
        }
        public string Id
        {
            get => m_Id;
            set => m_Id = value;
        }
        public ERarity Rarity
        {
            get => m_Rarity;
            set => m_Rarity = value;
        }
        public EShopWaveItem TypeItem
        {
            get => m_TypeItem;
            set => m_TypeItem = value;
        }

        public object GetObject()
        {
            switch (TypeItem)
            {
                case EShopWaveItem.Buff:
                    return DataManager.Base.Buff.GetBuffByType(Id);
                case EShopWaveItem.Equipment:
                    return DataManager.Base.Weapon.Get(Id);
                default:
                    return null;
            }
        }
        public virtual void SetupDetail(RectTransform rect) { }
    }

    public class ShopWaveBuffItem : ShopWaveItemModel
    {
        public override void SetupDetail(RectTransform rect)
        {
            var buff = (BuffEntity)GetObject();
            base.SetupDetail(rect);
            if (buff.LocalizeKey.IsNotNullAndEmpty())
            {

                return;
            }
            foreach (var mod in buff.ModifierPassive)
            {

            }
        }
    }

    public class ShopWaveWeaponItem : ShopWaveItemModel
    {
        public override void SetupDetail(RectTransform rect)
        {
            var equipment = (WeaponEntity)GetObject();
            base.SetupDetail(rect);

            // 
        }
    }
}
