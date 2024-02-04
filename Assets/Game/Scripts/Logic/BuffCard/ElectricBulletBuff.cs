using Cysharp.Threading.Tasks;
using Game.GameActor;
using Game.GameActor.Buff;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class ElectricBulletBuff : AbstractBuff
{
    public ValueConfigSearch TriggerCount;
    public WeaponBase Weapon;

    private WeaponBase weapon; 
    int count = 0;

    private void OnEnable()
    {
        Messenger.AddListener<Character>(EventKey.OnAttack, OnAttack);
    }

 

    private void OnDisable()
    {
        Messenger.RemoveListener<Character>(EventKey.OnAttack, OnAttack);
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        Messenger.RemoveListener<Character>(EventKey.OnAttack, OnAttack);
    }


    private void OnAttack(Character caster)
    {
        if (caster == Caster && weapon!=null)
        {
            count++;

            if (count >= TriggerCount.IntValue)
            {
                if (ReleaseBullet())
                {
                    count = 0;
                }
            }
        }
    }

    bool ReleaseBullet()
    {
        ITarget target = Caster.Sensor.CurrentTarget;
        if (target == null) return false;

        if (weapon.Trigger(
            Caster.GetMidTransform(), 
            target.GetMidTransform(),
            facing:(target.GetMidTransform().position- Caster.WeaponHandler.GetCurrentAttackPoint().position),
            trackingTarget:target,null)
            )
        {
            return true;
        }


        return false;
    }
    public override async void Play()
    {
        weapon=await Weapon.SetUp((Character)Caster);
    }
}