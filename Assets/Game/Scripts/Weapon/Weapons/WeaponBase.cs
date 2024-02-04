using com.assets.loader.addressables;
using com.assets.loader.core;
using Cysharp.Threading.Tasks;
using Game.GameActor;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class WeaponBase : ScriptableObject
{
    public delegate void OnReload(WeaponBase weapon);
    public delegate void OnStack(int stack);
    public OnReload onReload;
    public OnStack onStack;
    public int id;
    public string title;
    public int baseDamage;
    public ValueConfigSearch DamageMultiplier = new ValueConfigSearch(string.Empty, "1");
    public float damageMultiplier;
    public float fireRate;
    public float range = 10;
    public float lastTrigger = 0;
    [HideInInspector]
    public float bulletVelocity = 1;
    public ValueConfigSearch BulletVelocityConfig;

    public float knockBack = 0,knockBackTime=0.2f;
    public Character character;
    public string icon, mainTexture;
    public LayerMask mask;
    protected System.Action onEnded;
    [HideInInspector]
    private EquipableItem EquipableItem;
    private WeaponEntity WeaponEntity;


    public List<AssetReferenceGameObject> _weaponPassives = new List<AssetReferenceGameObject>();
    public List<WeaponBase> _supportWeapons = new List<WeaponBase>();

    public virtual async UniTask<WeaponBase> SetUp(Character character)
    {
        WeaponBase instance = (WeaponBase)CreateInstance(this.GetType().ToString());
        instance.name = title;
        instance.baseDamage = baseDamage;
        instance.character = character;
        instance.fireRate = fireRate;
        instance.DamageMultiplier = DamageMultiplier.Clone().SetId(character.gameObject.name);
        instance.damageMultiplier = DamageMultiplier.FloatValue;
        instance.range = range;
        instance.BulletVelocityConfig = BulletVelocityConfig.Clone().SetId(character.gameObject.name);
        instance.bulletVelocity = BulletVelocityConfig.FloatValue==0?bulletVelocity:instance.BulletVelocityConfig.FloatValue;
        instance.id = id;
        instance.icon = icon;
        instance.mainTexture = mainTexture;
        instance.knockBack = knockBack;
        instance.knockBackTime = knockBackTime;
        instance.mask = character.AttackHandler.targetMask;
        instance._weaponPassives = _weaponPassives;
        instance._supportWeapons = _supportWeapons;
        return instance;
    }

    public EquipableItem GetEquipableItem()
    {
        return EquipableItem;
    }
    public async UniTask SetItemEquipment(EquipableItem item)
    {
        if (item == null) return;
        EquipableItem = item;
        WeaponEntity = DataManager.Base.Weapon.Get(item.Id);
        foreach (var passive in _weaponPassives)
        {
            var passiveIns = (await Addressables.InstantiateAsync(passive, character.weaponHolder)).GetComponent<BaseEquipmentPassive>();
            passiveIns.SetEquipment(item);
            character.PassiveEngine.ApplyPassive(passiveIns);
        }
    }

    public virtual Transform GetShootPoint() { return null; }

    public void GetIcon(System.Action<Sprite> onLoad)
    {
        IAssetLoader loader = new AddressableAssetLoader();
        var request = loader.LoadAsync<Sprite>($"Gun/{icon}.png");
        request.Task.ContinueWith(sprite=> 
        {
            onLoad?.Invoke(sprite);

            loader.Release(request);
        }).Forget();
    }
    public void GetGunTexture(System.Action<Sprite> onLoad)
    {
        IAssetLoader loader = new AddressableAssetLoader();
        var request = loader.LoadAsync<Sprite>($"Gun/{mainTexture}.png");
        request.Task.ContinueWith(sprite =>
        {
            onLoad?.Invoke(sprite);

            loader.Release(request);
        }).Forget();
    }
    public virtual int GetDamage()
    {
        var multi = 1f;
        if (WeaponEntity != null)
        {
            multi = WeaponEntity.OutputDmg;
        }
        int value = (int)(character.Stats.GetValue(StatKey.Dmg) *multi);
        return (int)((value == 0 ? this.baseDamage : value)*damageMultiplier);
    }
    public float GetFireRate()
    {
        if (this.fireRate != 0) return fireRate;
        float value = character.Stats.GetValue(StatKey.FireRate);
        return value == 0?this.fireRate: value;
    }
    public float GetRange()
    {
        int value = (int)(character.Stats.GetValue(StatKey.Range));
        return value == 0 ? this.range : value;
    }
    public float GetBulletVelocity()
    {
        try
        {
            float value = EquipableItem != null ? EquipableItem.GetStat(StatKey.SpeedBullet).Value : 0;
            return value == 0 ? this.bulletVelocity : value;
        }
        catch
        {
            return this.bulletVelocity;

        }
    }

    public virtual void PlayTriggerSFX()
    {
       
    }
    public virtual void PlayShotSFX()
    {

    }
    public virtual void PlayTriggerImpact() { }
    public virtual bool CanTrigger()
    {
        return Time.time - lastTrigger >= (fireRate>=0?1 / GetFireRate():0);
    }
    public virtual bool Trigger(Transform triggerPos,Transform target,Vector2 facing, ITarget trackingTarget, System.Action onEnded)
    {
        return false;
    }
    //public virtual bool Trigger(Transform triggerPos, ITarget target, System.Action onEnded)
    //{
    //    return false;
    //}
    //public virtual bool Trigger(Transform transform, Action onAttackEnded)
    //{
    //    return false;
    //}
    //public virtual bool TriggerInstant(Transform transform, Action onAttackEnded)
    //{
    //    return false;
    //}
    public virtual void Release()
    {

    }
    public virtual void Destroy()
    {
        Logger.Log("=>>>DESTROY: " + name);
    }
    public virtual async UniTask Reload()
    {

    }
    public virtual void OnAttackEnded()
    {
        onEnded?.Invoke();
        onEnded = null;
    }

    //



}
