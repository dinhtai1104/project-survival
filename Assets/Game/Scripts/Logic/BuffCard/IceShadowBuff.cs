using com.assets.loader.addressables;
using com.assets.loader.core;
using Cysharp.Threading.Tasks;
using Game.GameActor;
using Game.GameActor.Buff;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class IceShadowBuff : AbstractBuff
{

    public ValueConfigSearch CoolDown,DamageMultiplier,FireRateMultiplier,Duration,MaxShadow;
    float coolDown;
    float time = 0;
    bool isReady = false;
    public AssetReferenceGameObject shadowVanishEffect;
    public AssetReferenceGameObject shadowAppearEffect;
    List<ShadowTimer> shadows = new List<ShadowTimer>();
    EHero CurrentHero;

    public class ShadowTimer
    {
        public ActorBase Shadow;
        public int Time;

        CancellationTokenSource cancellation;
        public ShadowTimer()
        {
            cancellation = new CancellationTokenSource();
        }
        public void Destroy()
        {
            if (cancellation != null)
            {
                cancellation.Cancel();
                cancellation.Dispose();
                cancellation = null;
            }
            if (Shadow != null)
            {
                Shadow.WeaponHandler.Destroy();
                Shadow.BehaviourHandler.Destroy();
                Shadow.PassiveEngine.RemovePassives();
                Shadow.SetActive(false);
            }

        }
        public async UniTask<ShadowTimer> SetUp()
        {
            float time = 0;
            while (time < Time)
            {
                time += GameTime.Controller.DeltaTime();
                await UniTask.Yield(cancellationToken:cancellation.Token);
            }
            return this;
        }
    }
    private void OnEnable()
    {
        coolDown = CoolDown.FloatValue;


        Messenger.AddListener<bool>(EventKey.GameClear, OnGameClear);
        Messenger.AddListener<Callback>(EventKey.StageStart, OnGameStart);
    }

    private void OnGameStart(Callback arg1)
    {
        isReady = true;
    }

    private void OnGameClear(bool arg1)
    {
        isReady = false;
        ClearShadow();

    }

    void ClearShadow()
    {
        for(int i = 0; i < shadows.Count; i++)
        {
            shadows[i].Destroy();
        }
        shadows.Clear();
    }
 

    private void OnDisable()
    {
        Messenger.RemoveListener<bool>(EventKey.GameClear, OnGameClear);
        Messenger.RemoveListener<Callback>(EventKey.StageStart, OnGameStart);
        ClearShadow();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        Messenger.RemoveListener<bool>(EventKey.GameClear, OnGameClear);
        Messenger.RemoveListener<Callback>(EventKey.StageStart, OnGameStart);
        ClearShadow();
    }
    IAssetLoader assetLoader = new AddressableAssetLoader();
    EquipmentHandler equipmentHandlerTrialHero;

    async UniTask PrepareShadow(EHero hero)
    {
        if (shadows.Count >= MaxShadow.IntValue)
        {
            //wont' reset timer to 0, set it close to cool down time
            time = CoolDown.FloatValue-2;
            return;
        }
        ActorBase shadow = (await Game.Pool.GameObjectSpawner.Instance.GetAsync(string.Format(AddressableName.Shadow, (int)(hero)), Game.Pool.EPool.Temporary)).GetComponent<ActorBase>();
        var mainWeapon = GameSceneManager.Instance.PlayerData.EquipmentHandler.GetEquipment(EEquipment.MainWeapon);

        WeaponBase playerWeapon = await assetLoader.LoadAsync<WeaponBase>("Weapon_" + mainWeapon.Id).Task;
        var weaponData = new WeaponData
        {
            Weapon = playerWeapon,
            Item = mainWeapon
        };
        shadow.WeaponHandler.ClearPool();
        shadow.WeaponHandler.LoadWeapon(playerWeapon, weaponData);
        var stats = GetStatPlayer();

        shadow.SetActive(false);

        await ((Character)shadow).SetUp(stats);

        shadow.AnimationHandler.SetSkin("-1");


        shadow.SetPosition(transform.position+new Vector3(-1,0));
        shadow.Input.SetActive(false);


        await UniTask.Yield();

      

        await ActiveShadow(shadow);

        var timer = new ShadowTimer() { Shadow=shadow,Time=Duration.IntValue};
        timer.SetUp().ContinueWith((timer)=> 
        {
            shadows.Remove(timer);
            Vector3 shadowPosition = timer.Shadow.GetPosition();
            Game.Pool.GameObjectSpawner.Instance.GetAsync(shadowVanishEffect.RuntimeKey.ToString()).ContinueWith(obj =>
            {
                var effect = obj.GetComponent<Game.Effect.EffectAbstract>();
                effect.Active(shadowPosition);
            }).Forget();

            timer.Destroy();
        }).Forget();

        shadows.Add(timer);
    }

    async UniTask ActiveShadow(ActorBase shadow)
    {
        Game.Pool.GameObjectSpawner.Instance.GetAsync(shadowAppearEffect.RuntimeKey.ToString()).ContinueWith(obj =>
        {
            var effect = obj.GetComponent<Game.Effect.EffectAbstract>();
            effect.Active(shadow.GetPosition()).SetParent(shadow.GetTransform());
        }).Forget();

        shadow.SetActive(true);

        equipmentHandlerTrialHero.EquipPassive(shadow,EEquipment.MainWeapon);

        shadow.PropertyHandler.AddProperty(EActorProperty.Trackable, 0);

       

        await UniTask.Delay(1000);
        shadow.StartBehaviours();

    }

    private IStatGroup GetStatPlayer()
    {
        CurrentHero = ((PlayerController)Caster).Hero;

        // create new stat for trial hero
        var heroCurrent = DataManager.Save.User.GetHero(CurrentHero);
        var stats = HeroFactory.Instance.GetHeroStat(CurrentHero, heroCurrent.Level, heroCurrent.Star);
        stats = HeroFactory.Instance.ApplyEquipment(stats, GameSceneManager.Instance.PlayerData.EquipmentHandler, out equipmentHandlerTrialHero);
        stats.AddModifier(StatKey.Dmg, new StatModifier(EStatMod.FlatMul, DamageMultiplier.FloatValue*(BuffData.Level+1)), this);
        stats.AddModifier(StatKey.FireRate, new StatModifier(EStatMod.FlatMul, FireRateMultiplier.FloatValue), this);
        stats.CalculateStats();
        return stats;
    }


    private void Update()
    {
        if (!isReady) return;
        if (time < coolDown)
        {
            time += GameTime.Controller.DeltaTime();
        }
        else
        {
            time = 0;
            CurrentHero = ((PlayerController)Caster).Hero;
            PrepareShadow(CurrentHero);
        }
    }
    public override void Play()
    {

    }
}