using Game.GameActor;
using System;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;

public class BoomerangPassive : WeaponPassive
{
    public ValueConfigSearch epic_subjutsuBoomerangDuration;
    public ValueConfigSearch epic_subjutsuBoomerangDmgMul;
    public ValueConfigSearch epic_subjutsuBoomerangDmgTickrate;
    
    public ValueConfigSearch legendary_subjutsuBoomerangDuration;

    public AssetReference shadowBoomerangRef;

    private void OnEnable()
    {
        Messenger.AddListener<ActorBase,List<ModifierSource>>(EventKey.OnBoomerangShoot, OnAttackEvent);
        Messenger.AddListener<BulletBase, ITarget>(EventKey.OnBulletImpact, OnBulletImpact);
    }

    private void OnAttackEvent(ActorBase actor, List<ModifierSource> modifiers)
    {
        if (actor != Caster) return;

        if ((int)Rarity >= (int)ERarity.Epic)
        {
            //Logger.Log("EPIC");
            modifiers[1].AddModifier(new StatModifier(EStatMod.Flat, epic_subjutsuBoomerangDmgTickrate.FloatValue));
            modifiers[2].AddModifier(new StatModifier(EStatMod.Flat, epic_subjutsuBoomerangDuration.FloatValue));
            modifiers[3].AddModifier(new StatModifier(EStatMod.Flat, epic_subjutsuBoomerangDmgMul.FloatValue));
        }
        if ((int)Rarity >= (int)ERarity.Legendary)
        {
            //Logger.Log("Legendary");
            modifiers[2].AddModifier(new StatModifier(EStatMod.Flat, legendary_subjutsuBoomerangDuration.FloatValue));
        }

        //foreach(var mod in modifiers)
        //{
        //    mod.Stat.RecalculateValue();
        //    Logger.Log(mod.Stat.BaseValue + " => " + mod.Value);
        //}
    }
    void OnBulletImpact(BulletBase bullet,ITarget target )
    {
        if (Caster!=bullet.Caster) return;
        if ( target==null||(target != null && (Object)target == Caster) || (target != null && target.IsDead()))
        {
            return;
        }
        if (target != null && !Caster.AttackHandler.IsValid(target.GetCharacterType()))
        {
            return;
        }


        if ((int)Rarity >= (int)ERarity.Epic)
        {
            Game.Pool.GameObjectSpawner.Instance.Get(shadowBoomerangRef.RuntimeKey.ToString(), obj =>
            {
                var shadow = obj.GetComponent<ShadowBoomerangImpactHandler>();
                shadow.SetUp(bullet);
            }, Game.Pool.EPool.Projectile);
        }
       

    }
    private void OnDisable()
    {
        Messenger.RemoveListener<ActorBase, List<ModifierSource>>(EventKey.OnBoomerangShoot, OnAttackEvent);
        Messenger.RemoveListener<BulletBase, ITarget>(EventKey.OnBulletImpact, OnBulletImpact);
    }

  
}
