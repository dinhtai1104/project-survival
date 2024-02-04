using CodeStage.AntiCheat.ObscuredTypes;
using System;
using UnityEngine;

namespace Game.GameActor
{
    [System.Serializable]
    public abstract class HealthHandler : IHealthHandler
    {
        public ActorBase Actor { get; set; }

        public delegate void OnUpdate(HealthHandler health);
        public OnUpdate onUpdate;

        public delegate void OnHealthDepleted();
        public OnHealthDepleted onHealthDepleted;
        public delegate void OnArmorBroken();
        public OnArmorBroken onArmorBroke;

        private ObscuredFloat healthPoint, armorPoint;
        private ObscuredFloat maxHP, defaultHP;

        public HealthHandler(float health, float armor)
        {
            defaultHP = maxHP = healthPoint = health;
            armorPoint = armor;

        }

        public void Reset(float health, float armor)
        {
            defaultHP = maxHP = healthPoint = health;
            armorPoint = armor;
            onUpdate?.Invoke(this);
        }

        public void SetActor(ActorBase actor)
        {
            this.Actor = actor;
            Actor.Stats.AddListener(StatKey.Hp, OnChangeMaxHp);
        }

        ~HealthHandler()
        {
            Actor.Stats.RemoveListener(StatKey.Hp, OnChangeMaxHp);
        }
        private void OnChangeMaxHp(float newValue)
        {
            int newHp = (int)newValue;
            var change = newHp - maxHP;
            maxHP = newHp;

            //// Add Heal Effect
            //if (Actor != null)
            //{
            //    Actor.Heal(change);
            //}
        }

        public virtual ObscuredFloat GetArmor()
        {
            return armorPoint;
        }

        public virtual ObscuredFloat GetHealth()
        {
            return healthPoint;
        }

        public virtual ObscuredFloat GetPercentHealth()
        {
            return healthPoint * 1.0f / maxHP;
        }
        public ObscuredFloat GetMaxHP()
        {
            return maxHP;
        }
        public void AddArmor(int point)
        {
            SetArmor((int)GetArmor() + point);
        }

        public void ResetArmor()
        {
            armorPoint = 0;
            ArmorDepleted();
            onUpdate?.Invoke(this);
        }

        public void SetArmor(int armor)
        {
            if (armor <= 0)
            {
                if (this.armorPoint <= 0) return;
            }
            this.armorPoint = armor;
            this.armorPoint = this.armorPoint <= 0 ? 0 : (int)this.armorPoint;
            if (armorPoint == 0)
            {
                ArmorDepleted();
            }
            onUpdate?.Invoke(this);
        }
        public void AddHealth(float point)
        {
            SetHealth(GetHealth() + point);
        }
        public void SetHealth(float health)
        {
            if (GetHealth() <= 0 || GetHealth() > GetMaxHP()) return;
            this.healthPoint = health;
            this.healthPoint = Mathf.Clamp(this.healthPoint, 0, GetMaxHP());
            onUpdate?.Invoke(this);
            if (GetHealth() == 0)
            {
                HealthDepleted();
            }
        }
        public void SetMaxHealth(float newValue)
        {
            maxHP = newValue;
        }
        public void HealthDepleted()
        {
            onHealthDepleted?.Invoke();
            Actor.Stats.RemoveListener(StatKey.Hp, OnChangeMaxHp);
        }

        public void ArmorDepleted()
        {
            onArmorBroke?.Invoke();

        }

        public void RestoreHealth()
        {
            healthPoint = maxHP;
            SetHealth(maxHP);
        }
    }
}