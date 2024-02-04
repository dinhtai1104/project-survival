using ExtensionKit;
using System;
using UnityEngine;

namespace Game.GameActor
{
    public class DamageHandler : MonoBehaviour, IDamageHandler
    {
        private const float MIN_DAMAGE_CAP = 0;
        private const float MAX_DAMAGE_CAP = 2;
        public ValueConfigSearch getHitSpacing = new ValueConfigSearch(null, "0");
        private float lastGetHitTime = 0;
        public bool GetHit(IDamageDealer damageDealer, DamageSource damageSource)
        {
            if (Time.time - lastGetHitTime < getHitSpacing.FloatValue || damageSource.Value == 0) return false;

            //Logger.Log("5GET HIT:" + damageSource.Attacker.gameObject.name + " > " + damageSource.Defender.gameObject.name + " " + damageSource._damage.Value + " " + current.Stats.GetStat(StatKey.ReduceDmg).Value);


            ActorBase current = damageSource.Defender;
            ActorBase attacker = damageSource.Attacker;
            if (current.PropertyHandler.GetProperty(EActorProperty.Dead, 0) == 1 || current.PropertyHandler.GetProperty(EActorProperty.Vunerable, 0) == 0) return false;
            damageSource._damage.AddModifier(new StatModifier(EStatMod.PercentMul, -current.Stats.GetStat(StatKey.ReduceDmg).Value));

            CalculateExtraDmg(damageSource);

            if (attacker != null)
            {
                // calculate crit

                //headshot valid
                if (!damageSource.Defender.Tagger.HasTag(ETag.Boss) && !damageSource.Defender.Tagger.HasTag(ETag.MiniBoss))
                {
                    if (GameUtility.GameUtility.RandomBoolean(attacker.Stats.GetValue(StatKey.HeadshotRate)))
                    {
                        damageSource._damageType = EDamage.HeadShot;
                        damageSource.Value = damageSource.Defender.GetHealthPoint();
                    }
                }

                //defender dodge bullet
                if (GameUtility.GameUtility.RandomBoolean(current.Stats.GetValue(StatKey.DodgeRate)))
                {
                    damageSource._damageType = EDamage.Missed;
                    damageSource.Value = 0;
                }
            }
            damageSource._damage.RecalculateValue();
            // push event before hit to get modifier data damagesource
            Messenger.Broadcast(EventKey.BeforeHit, attacker, current, damageSource);
            if (attacker != null)
            {
                if (damageSource.IsCrit || GameUtility.GameUtility.RandomBoolean(attacker.Stats.GetValue(StatKey.CritRate)))
                {
                    damageSource._damageType = EDamage.Critital;
                    damageSource.Value *= attacker.Stats.GetValue(StatKey.CritDmg);
                }
            }

            damageSource._damage.RecalculateValue();
            float baseDamage = damageSource.Value;

            // cap max 
            damageSource._damage.SetConstraintMax(MAX_DAMAGE_CAP * baseDamage);
            damageSource._damage.SetConstraintMin(MIN_DAMAGE_CAP * baseDamage);

            damageSource._damage.RecalculateValue();

            var newDamage = (int)damageSource.Value;

            if (current.HealthHandler.GetArmor() > 0)
            {
                current.HealthHandler.AddArmor(-newDamage);
            }
            else
            {
                current.HealthHandler.AddHealth(-newDamage);
            }

            //Shake().Forget();
            if (current.HealthHandler.GetHealth() > 0)
            {
                if (damageSource._damageSource.Is(EDamageSource.Weapon))
                {
                    Messenger.Broadcast(EventKey.AttackEventByWeapon, attacker, current);
                }
                else
                {
                    Messenger.Broadcast(EventKey.AttackEvent, attacker, current);
                }
                current.AnimationHandler.SetGetHit();
            }
            else
            {
                Messenger.Broadcast(EventKey.KilledBy, attacker, current);
            }
            lastGetHitTime = Time.time;

            return true;

        }

        private void CalculateExtraDmg(DamageSource damageSource)
        {
            var attacker = damageSource.Attacker;
            var defender = damageSource.Defender;

            if (attacker == null) return;
            if (defender == null) return;

            if (!defender.Tagger.HasTag(ETag.Enemy)) return;

            // if enemy is boss enemy
            if (defender.Tagger.HasAnyTags(ETag.Boss, ETag.MiniBoss))
            {
                damageSource.AddModifier(new StatModifier(EStatMod.PercentAdd, attacker.GetStatValue(StatKey.DmgIncreaseBoss)));
            }
            // if enemy is melee enemy
            if (defender.Tagger.HasTag(ETag.Melee))
            {
                damageSource.AddModifier(new StatModifier(EStatMod.PercentAdd, attacker.GetStatValue(StatKey.DmgIncreaseMeleeEnemy)));
            }
            // if enemy is range enemy
            if (defender.Tagger.HasTag(ETag.Range))
            {
                damageSource.AddModifier(new StatModifier(EStatMod.PercentAdd, attacker.GetStatValue(StatKey.DmgIncreaseRangeEnemy)));
            }
        }
    }
}