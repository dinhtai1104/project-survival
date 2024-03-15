using Assets.Game.Scripts.Core.Data.Database.Equipment.Weapon;
using Assets.Game.Scripts.Core.Data.Database.ShopWave;
using Cysharp.Threading.Tasks;
using Engine;
using Framework;
using GameUtility;
using SceneManger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Game.Scripts.GameScene.ShopWave
{
    [System.Serializable]
    public class ShopWaveHandler : IDisposable
    {
        private BaseSceneController m_SceneController;
        private BaseScenePresenter m_ScenePresenter => m_SceneController.ScenePresenter;
        private int m_CurrentWave;
        private int m_CurrentRerollNumber;
        private float m_ShopPrice = 1f;
        private ShopWaveConfigTable m_ShopWaveConfigTable;

        private IStatGroup m_Stats => m_Actor.Stats;
        private PlayerActor m_Actor;
        public ShopWaveHandler() { }
        public ShopWaveHandler(BaseSceneController sceneController, PlayerActor actor) : this()
        {
            m_SceneController = sceneController;
            this.m_Actor = actor;
            m_ShopWaveConfigTable = DataManager.Base.ShopWaveConfig;
            m_Stats.AddListener(StatKey.ItemPrice, OnChangeItemPrice);
        }


        #region Handle Shop Wave

        public void SetWave(int currentWave)
        {
            m_CurrentWave = currentWave;
        }

        public List<ShopWaveItemModel> RequestShopWave(int amount)
        {
            var rs = new List<ShopWaveItemModel>();

            for (int i = 0; i < amount; i++)
            {
                rs.Add(RequestItemModel());
            }

            return rs;
        }

        private ShopWaveItemModel RequestItemModel()
        {
            var model = new ShopWaveItemModel();

            // Random type
            if (MathUtils.RandomBoolean(0.65f))
            {
                model.TypeItem = EShopWaveItem.Buff;
                model = RequestBuff(model);
            }
            else
            {
                model.TypeItem = EShopWaveItem.Equipment;
                model = RequestEquipment(model);
            }

            return model;
        }

        private ShopWaveItemModel RequestBuff(ShopWaveItemModel model)
        {
            // Random Rarity
            var chanceConfig = m_ShopWaveConfigTable.Get(model.TypeItem);
            var chancesPerRarity = CreateChance(chanceConfig, ERarity.Common, ERarity.Epic);
            var rarityChance = chancesPerRarity.RandomWeighted(out var index);
            model.Rarity = rarityChance.Rarity;

            // Random Id
            var buffDatabase = DataManager.Base.Buff;
            var filter = buffDatabase.FilterByRarity(model.Rarity);
            var buff = filter.RandomWeighted(out index);
            model.Id = buff.Type;
            model.Price = buff.Price;

            return model;
        }
        private ShopWaveItemModel RequestEquipment(ShopWaveItemModel model)
        {
            var groupEquipment = new List<ChancePerGroupEquipment>()
            {
                new ChancePerGroupEquipment() { Type = EGroupEquipment.AllType, Chance = 65 },
                new ChancePerGroupEquipment() { Type = EGroupEquipment.Class, Chance = 15 },
                new ChancePerGroupEquipment() { Type = EGroupEquipment.Own, Chance = 20 },
            };

            var groupEquipmentPicked = groupEquipment.RandomWeighted(out var index);
            var weaponRd = m_Actor.WeaponHolder.ListWeaponCurrent.Random().WpEntity;

            switch (groupEquipmentPicked.Type)
            {
                case EGroupEquipment.AllType:
                    var wq = DataManager.Base.Weapon.GetRandomWeight();
                    model.Id = wq.IdEquipment;
                    break;
                case EGroupEquipment.Class:
                    model.Id = DataManager.Base.Weapon.GetRandomWeightByClass(weaponRd.WeaponClass);
                    break;
                case EGroupEquipment.Own:
                    model.Id = weaponRd.IdEquipment;
                    break;
            }


            // Random Rarity
            var chanceConfig = m_ShopWaveConfigTable.Get(model.TypeItem);
            var chancesPerRarity = CreateChance(chanceConfig);
            var rarityChance = chancesPerRarity.RandomWeighted(out index);
            model.Rarity = rarityChance.Rarity;

            var entity = DataManager.Base.Weapon.Get(model.Id)[model.Rarity];
            model.Price = entity.Price;

            return model;
        }
        #endregion



        private void OnChangeItemPrice(float statValue)
        {
            // Reduce shop price
            m_ShopPrice = 1 + statValue;
        }

        public void Dispose()
        {
            m_Stats.RemoveListener(StatKey.ItemPrice, OnChangeItemPrice);
        }


        private List<ChancePerRarity> CreateChance(Dictionary<ERarity, ShopWaveConfigEntity> chanceConfig, ERarity Min = ERarity.Common, ERarity Max = ERarity.Ultimate)
        {
            var result = new List<ChancePerRarity>();
            float lastChance = 0;
            for (ERarity rarity = Max; rarity >= Min; rarity--)
            {
                var chance = new ChancePerRarity();
                chance.Rarity = rarity;
                var config = chanceConfig[rarity];

                if (m_CurrentWave >= config.MinWaveUnlock)
                {
                    var luck = m_Stats.GetValue(StatKey.LuckRate);
                    // Calculate chance rarity
                    chance.Chance = (m_CurrentWave - config.MinWaveUnlock + 1) * (config.ChancePerWave + luck) * (1 - lastChance);

                    if (chance.Chance > config.MaxChance)
                    {
                        chance.Chance = config.MaxChance;
                    }
                }

                lastChance += chance.Chance;
                result.Add(chance);
            }

            return result;
        }


        class ChancePerRarity : IWeightable
        {
            public ERarity Rarity;
            public float Chance = 0;

            public float Weight => Chance;
        }

        class ChancePerGroupEquipment : IWeightable
        {
            public float Chance = 0;
            public EGroupEquipment Type;
            public float Weight => Chance;
        }

        enum EGroupEquipment
        {
            AllType,
            Class,
            Own,
        }
    }
}
