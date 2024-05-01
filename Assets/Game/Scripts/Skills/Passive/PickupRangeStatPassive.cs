using Assets.Game.Scripts.Manager;
using DG.Tweening;
using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Game.Scripts.Skills.Passive
{
    public class PickupRangeStatPassive : BasePassive
    {
        private float m_PickupRange = 0;
        public override void Equip()
        {
            base.Equip();
            m_PickupRange = Owner.Stats.GetValue(StatKey.PickupRange, 0);
            Owner.Stats.AddListener(StatKey.PickupRange, OnChangePickupRange);
        }
        public override void UnEquip()
        {
            base.UnEquip();
            Owner.Stats.RemoveListener(StatKey.PickupRange, OnChangePickupRange);
        }

        private void OnChangePickupRange(float statValue)
        {
            m_PickupRange = statValue;
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            if (Owner == null) return;
            if (LootObjectUpdater.Instance == null) return;
            var getNearestLootable = LootObjectUpdater.Instance.GetNearest(Owner.CenterPosition, m_PickupRange);
            if (getNearestLootable)
            {
                getNearestLootable.Loot();
                getNearestLootable.transform.DOMove(Owner.CenterPosition, 0.3f)
                    .OnComplete(() =>
                    {
                        getNearestLootable.Destroy();
                    });
            }
        }
    }
}
