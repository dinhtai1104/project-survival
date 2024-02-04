using com.mec;
using Game.GameActor;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ElectricBallObject : CharacterDamageObject, IRadiusRequire
{
    private CoroutineHandle updateHandler;
    public AttackCircleCast2D attackCircleCast2D;
    public LayerMask layerMask;
    public LineSimpleControl thunderPrefab;
    public Stat Radius { set; get; }
    private int MaxTarget;
    private float Cooldown;

    public override void Play()
    {
        attackCircleCast2D = new AttackCircleCast2D(Caster, MaxTarget);
        base.Play();
        Timing.KillCoroutines(updateHandler);
        updateHandler = Timing.RunCoroutine(_UpdateHandler());
        attackCircleCast2D.OnSuccess -= OnAttack;
        attackCircleCast2D.OnSuccess += OnAttack;
    }

    private void OnAttack(ActorBase target)
    {
        //Debug.Log("Release Thunder To: " + target.name);
        var thunder = PoolManager.Instance.Spawn(thunderPrefab);
        thunder.SetPos(0, transform.position);
        thunder.SetPos(1, target.GetMidTransform().position);

        var dmg = new DamageSource
        {
            Attacker = Caster,
            Defender = target,
            _damage = DmgStat,
        };
        dmg.posHit = transform.position;
        dmg._damageSource = EDamageSource.Effect;
        target.GetHit(dmg, target);
    }

    private IEnumerator<float> _UpdateHandler()
    {
        while (true)
        {
            attackCircleCast2D.DealDamage(transform.position, Vector3.zero, Radius.Value, layerMask);
            yield return Timing.WaitForSeconds(Cooldown);
        }
    }

    private void OnDisable()
    {
        attackCircleCast2D.OnSuccess -= OnAttack;
        Timing.KillCoroutines(updateHandler);
    }

    public void SetMaxTarget(int ballMaxTarget)
    {
        this.MaxTarget = ballMaxTarget;
    }

    public void SetCooldown(float cooldown)
    {
        this.Cooldown = cooldown;
    }
}