using Cysharp.Threading.Tasks;
using Game.GameActor;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

[CreateAssetMenu (menuName = "Gun/LaserGun")]
public class LaserGun : GunBase
{
    public AssetReferenceGameObject laserBeamRef;
    LaserBeam laserBeam;

    public ValueConfigSearch ChargeTime=new ValueConfigSearch(string.Empty,"1.5");
    public override async UniTask<WeaponBase> SetUp(Character character)
    {
        LaserGun instance = (LaserGun)await base.SetUp(character);
        if (laserBeamRef.RuntimeKeyIsValid())
        {
            instance.laserBeam = (await Addressables.InstantiateAsync(laserBeamRef, character.weaponHolder,instantiateInWorldSpace:true)).GetComponent<LaserBeam>();
            instance.laserBeam.transform.localPosition = Vector3.zero;
            instance.laserBeam.gameObject.SetActive(false);
        }
        instance.ChargeTime = ChargeTime.Clone().SetId(character.gameObject.name);
        return instance;
    }
    public override bool Trigger(Transform triggerPos, Transform target, Vector2 facing, ITarget trackingTarget, System.Action onEnded)
    {
        return TriggerLaser(triggerPos,facing);
    }
    //public override bool Trigger(Transform triggerPos, ITarget target, System.Action onEnded)
    //{
    //    return TriggerLaser(triggerPos,(target.GetMidTransform().position-triggerPos.position).normalized);
    //}


    //
    public  bool TriggerLaser(Transform triggerPos,Vector2 direction)
    {
        if (Time.time - lastTrigger >= 1 / GetFireRate())
        {
           
            if (weaponObj != null)
                weaponObj.Trigger();

            lastTrigger = Time.time;



            if (!laserBeam.isActive)
            {
                if (fireSFXs != null && fireSFXs.Length > 0)
                {
                    character.SoundHandler.PlayOneShot(fireSFXs[Random.Range(0, fireSFXs.Length)], 0.4f);
                }

                laserBeam.SetUp(this, triggerPos, fireRate,chargeTime:ChargeTime.FloatValue);
            }
            else if (!laserBeam.isStarted)
            {
                laserBeam.SetDirection(direction);
            }
            else
            {
                ITarget target = laserBeam.GetTarget();
                if (target != null && !target.IsDead())
                {
                    DamageSource damageSource = new DamageSource(character, (ActorBase)target, this.GetDamage(),character);
                    damageSource._damageSource = EDamageSource.Weapon;
                    target.GetHit(damageSource,  character);
                }
                else
                {
                    lastTrigger = 0;
                }
            }

            OnAttackEnded();

            return true;
        }
        return false;
    }
    public override void Release()
    {
        base.Release();
        if (laserBeam.isActive)
        {
            laserBeam.Stop();
        }
    }
    public override void Destroy()
    {
        base.Destroy();
        if (laserBeam != null)
        {
            Addressables.ReleaseInstance(laserBeam.gameObject);
        }
    }
}