using Game.GameActor;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class ElectricBallistaPassive : WeaponPassive
{
    [Header("Stack")]
    public ValueConfigSearch default_TimeFullStack;
    public ValueConfigSearch epic_TimeFullStack;

    public AssetReference muzzleEffect;

    private bool canAddStack = true;
    private float timeFullStack = 0;
    private float timeCurrentStack = 0;


    [Header("Ball Electric")]
    public ElectricBallObject electricBallPrefab;
    public ValueConfigSearch size_ball_Electric;
    public ValueConfigSearch default_ball_timeCooldownEachReleaseThunder;
    public ValueConfigSearch default_ball_radiusAffect;
    public ValueConfigSearch default_ball_maxTargetThunderAffect;
    public ValueConfigSearch legendary_ball_maxTargerThunderAffect;
    public ValueConfigSearch default_ball_dmgMulThunder;
    public ValueConfigSearch default_ball_speedMove;

    private float ballElectricSize;
    private float ballTimeCooldownThunder;
    private float ballRadiusAffect;
    private float thunderDmgMul;
    private float ballSpeedMove;
    private int ballMaxTarget;
    private bool isRelasedBall = false;

    private void Start()
    {
        Messenger.AddListener<Callback>(EventKey.StageStart, OnStageStart);
    }
    private void OnDestroy()
    {
        Messenger.RemoveListener<Callback>(EventKey.StageStart, OnStageStart);
    }

    private void OnStageStart(Callback cb)
    {
        timeCurrentStack = 0f;
        canAddStack = true;
    }
    private void OnDisable()
    {
        timeCurrentStack = 0;
    }

    public override void Play()
    {
        base.Play();
        timeCurrentStack = 0;
        timeFullStack = Rarity >= ERarity.Epic ? epic_TimeFullStack.FloatValue : default_TimeFullStack.FloatValue;

        ballElectricSize = size_ball_Electric.FloatValue;
        ballTimeCooldownThunder = default_ball_timeCooldownEachReleaseThunder.FloatValue;
        ballRadiusAffect = default_ball_radiusAffect.FloatValue;
        thunderDmgMul = default_ball_dmgMulThunder.FloatValue;
        ballSpeedMove = default_ball_speedMove.FloatValue;
        ballMaxTarget = Rarity >= ERarity.Legendary ? legendary_ball_maxTargerThunderAffect.IntValue : default_ball_maxTargetThunderAffect.IntValue;
    }


    public override void OnUpdate()
    {
        base.OnUpdate();
        if (!canAddStack) return;
        timeCurrentStack += Time.deltaTime;
        if (timeCurrentStack >= timeFullStack)
        {
            canAddStack = false;
            ReleaseElectricBall();
        }
    }

    private void ReleaseElectricBall()
    {
        if (Caster.FindClosetTarget() != null)
        {
            var directionActor = (Caster.Sensor.CurrentTarget.GetMidTransform().position- Caster.WeaponHandler.GetCurrentAttackPoint().position).normalized;
            var ball = PoolManager.Instance.Spawn(electricBallPrefab);
            ball.transform.position = Caster.WeaponHandler.GetCurrentAttackPoint().position;
            var speedMove = new Stat(ballSpeedMove);
            var moveComponent = ball.Movement;

            moveComponent.Speed = speedMove;
            ball.SetCaster(Caster);
            ball.transform.localScale = Vector3.one * ballElectricSize;
            ball.DmgStat = new Stat(Caster.Stats.GetValue(StatKey.Dmg) * thunderDmgMul);
            ball.SetMaxTarget(ballMaxTarget);
            ball.SetCooldown(ballTimeCooldownThunder);
            ball.Radius = new Stat(ballRadiusAffect);
            ball.transform.right = directionActor;
            ball.Play();
            ball._hit.onTrigger += OnTrigger;

            Game.Pool.GameObjectSpawner.Instance.Get(muzzleEffect.RuntimeKey.ToString(), obj => 
            {
                obj.GetComponent<Game.Effect.EffectAbstract>().Active(Caster.WeaponHandler.GetCurrentAttackPoint().position);
            });
        }
        else
        {
            canAddStack = true;
        }
    }

    private void OnTrigger(Collider2D collider, ITarget target)
    {
        timeCurrentStack = 0f;
        canAddStack = true;
    }
}
