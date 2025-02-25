using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Game.Scripts.Skills.Passive
{
    public class HpRegenerationStatPassive : BasePassive
    {
        private float time = 0;
        private float timeHeal = 0;
        private float hpHeal = 0;
        public override void Equip()
        {
            base.Equip();
            timeHeal = 1f;
            hpHeal = Owner.Stats.GetValue(StatKey.HpRegeneration);
            Owner.Stats.AddListener(StatKey.HpRegeneration, OnChangeHpRegeneration);
        }
        public override void OnUpdate()
        {
            base.OnUpdate();
            time += Time.deltaTime;
            if (time > timeHeal)
            {
                time = 0;
                Healing();
            }
        }

        private void Healing()
        {
            Owner.Health.Healing(hpHeal);
            Debug.Log("Hp Regeneration ----> " + Owner.name);
        }

        public override void UnEquip()
        {
            base.UnEquip();
            Owner.Stats.RemoveListener(StatKey.HpRegeneration, OnChangeHpRegeneration);
            Owner = null;
        }

        private void OnChangeHpRegeneration(float statValue)
        {
            hpHeal = statValue;
        }
    }
}
