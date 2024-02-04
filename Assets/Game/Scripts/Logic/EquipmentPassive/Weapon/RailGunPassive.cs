using com.mec;
using Cysharp.Threading.Tasks;
using Game.GameActor;
using GameUtility;
using System;
using System.Threading;

public class RailGunPassive : WeaponPassive
{
    public LineSimpleControl lazerPredictNoThoughGroundPrefab;
    public LineSimpleControl lazerPredicThoughGroundtPrefab;

    public LazerObject lazerNotThroughGround;
    public LazerObject lazerThroughGround;

    public ValueConfigSearch maxStack;
    public ValueConfigSearch lazerDmgMul;

    public ValueConfigSearch epic_lazerDmgMul;
    public ValueConfigSearch waitAfterReleaseLazer;
    public ValueConfigSearch lazerDmgInterval;
    public ValueConfigSearch lazerDuration;

    private int currentStack = 0;
    private bool canAddStack = true;
    private CancellationTokenSource token;
    public override void Initialize(ActorBase actor)
    {
        base.Initialize(actor);
        token = new CancellationTokenSource();
        Caster.AttackHandler.onShoot += OnShootCallback;
    }
    public override void Remove()
    {
        Caster.AttackHandler.onShoot -= OnShootCallback;
        base.Remove();
    }
    private void OnEnable()
    {
        Messenger.AddListener<Callback>(EventKey.StageStart, OnStageStart);
    }
    private void OnDisable()
    {
        Messenger.RemoveListener<Callback>(EventKey.StageStart, OnStageStart);
    }

    private void OnStageStart(Callback cb)
    {
        token.Cancel();
        token.Dispose();
        token = new CancellationTokenSource();
        Caster.AttackHandler.active = true;
        canAddStack = true;
        currentStack = 0;
    }

    private void OnShootCallback(Character character)
    {
        if (!canAddStack) return;
        currentStack++;
        Logger.Log("[RailGun] CurrentStack:: " + currentStack);
        if (currentStack == maxStack.IntValue)
        {
            currentStack = 0;
            Logger.Log("[RailGun] Release Lazer:: ");
            canAddStack = false;
            ReleaseLazer();
        }
    }
    LazerObject lz;

    private async void ReleaseLazer()
    {
        var nearestEnemy = Caster.FindClosetTarget();
        if (nearestEnemy != null)
        {
            Caster.AttackHandler.active = false;
            //var angle = GameUtility.GameUtility.GetAngle(Caster, nearestEnemy);

            ////Predict
            //var lazerPredictIns = Rarity >= ERarity.Legendary ? lazerPredicThoughGroundtPrefab : lazerPredictNoThoughGroundPrefab;
            //var lzPredict = PoolManager.Instance.Spawn(lazerPredictIns, Caster.WeaponHandler.transform);
            //lzPredict.transform.localPosition = UnityEngine.Vector3.zero;
            //lzPredict.transform.eulerAngles = new UnityEngine.Vector3(0, 0, angle);

            //await UniTask.Delay(500, cancellationToken: token.Token);
            //PoolManager.Instance.Despawn(lzPredict.gameObject);

            var lazerIns = Rarity >= ERarity.Legendary ? lazerThroughGround : lazerNotThroughGround;
            lz = PoolManager.Instance.Spawn(lazerIns, Caster.WeaponHandler.GetCurrentAttackPoint());
            lz.transform.localPosition = UnityEngine.Vector3.zero;
            lz.SetCaster(Caster);
            lz.SetMaxHit(-1);
            
            lz._hit.SetIntervalTime(lazerDmgInterval.FloatValue);
            float dmg = 0;
            if (Rarity >= ERarity.Epic)
            {
                dmg = epic_lazerDmgMul.FloatValue * Caster.Stats.GetValue(StatKey.Dmg);
            }
            else
            {
                dmg = lazerDmgMul.FloatValue * Caster.Stats.GetValue(StatKey.Dmg);
            }

            lz.Play(0, new Stat(dmg), lazerDuration.FloatValue);
            lz.SetDirection((Caster.Sensor.CurrentTarget.GetMidTransform().position - Caster.WeaponHandler.GetCurrentAttackPoint().position));
            lz.SetOnDestroyCallback(OnAttackLazerEnd);
        }
        else
        {
            canAddStack = true;
        }
    }
    public override void OnUpdate()
    {
        base.OnUpdate();
        if (lz != null &&Caster.Sensor.CurrentTarget!=null)
        {
            lz.SetDirection((Caster.Sensor.CurrentTarget.GetMidTransform().position - Caster.WeaponHandler.GetCurrentAttackPoint().position));
        }
    }
    private async void OnAttackLazerEnd()
    {
        await UniTask.Delay((int)(waitAfterReleaseLazer.FloatValue * 1000), cancellationToken: token.Token);
        Caster.AttackHandler.active = true;
        canAddStack = true;
    }
}
