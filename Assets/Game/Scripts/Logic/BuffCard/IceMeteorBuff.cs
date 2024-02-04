using Cysharp.Threading.Tasks;
using Game.GameActor;
using Game.GameActor.Buff;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class IceMeteorBuff : AbstractBuff
{

    public ValueConfigSearch CoolDown,DamageMultiplier;
    float coolDown;
    [SerializeField]
    private AssetReference shooterRef;
    SkyShooter shooter;
    float time = 0;
    bool isReady = false;
    private void OnEnable()
    {
        isReady = false;
        coolDown = CoolDown.FloatValue;

        Game.Pool.GameObjectSpawner.Instance.Get(shooterRef.RuntimeKey.ToString(), obj =>
         {
             SetUpSkyShooter(obj.GetComponent<SkyShooter>()).Forget();
         }, Game.Pool.EPool.Pernament);

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

    }

    async UniTask SetUpSkyShooter(SkyShooter shooter)
    {
        this.shooter = shooter;
        await shooter.SetUp(GetStat());
        shooter.SetActive(true);
        shooter.SetPosition(new Vector3(0, 11));

    }
    protected  IStatGroup GetStat()
    {
        var playerStats = GameSceneManager.Instance.PlayerData.Stats;
        var stats = DroneStat.Default();
        stats.SetBaseValue(StatKey.Dmg, playerStats.GetStat(StatKey.Dmg).Value);

        // drone attack dmg increase 100%
        stats.AddModifier(StatKey.Dmg, new StatModifier(EStatMod.PercentAdd, DamageMultiplier.FloatValue), this);


        stats.CalculateStats();
        return stats;
    }

    private void OnDisable()
    {
        Messenger.RemoveListener<bool>(EventKey.GameClear, OnGameClear);
        Messenger.RemoveListener<Callback>(EventKey.StageStart, OnGameStart);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        Messenger.RemoveListener<bool>(EventKey.GameClear, OnGameClear);
        Messenger.RemoveListener<Callback>(EventKey.StageStart, OnGameStart);
    }
    bool TriggerAttack()
    {
        ITarget target = shooter.Sensor.CurrentTarget;
        if (target != null)
        {
            shooter.AttackHandler.Trigger((target.GetMidTransform().position-shooter.WeaponHandler.GetCurrentAttackPoint().position),target);
            return true;
        }
        return false;
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
            //if attack is valid
            if (TriggerAttack())
            {
                time = 0;
            }
            //if no target is found, set cooldown 1s
            else
            {
                time = coolDown - 1;
            }
        }
    }
    public override void Play()
    {
    }
}