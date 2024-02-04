using Game.GameActor;
using System;
using UnityEngine;

[RequireComponent(typeof(InvokeHitByTrigger))]
public class PoisonObject : CharacterObjectBase, IPoison
{
    public InvokeHitByTrigger _hit;
    private object source;
    public Stat Dmg { set; get; }
    public Stat IntervalTime { set; get; }
    public Stat Duration { set; get; }
    private bool isStart = false;

    private void OnValidate()
    {
        _hit = GetComponent<InvokeHitByTrigger>();
    }

    private void OnEnable()
    {
        _hit.onTrigger += OnTriggerHit;
    }

    public void SetSource(object source)
    {
        this.source = source;
    }

    public void SetInterval(Stat interval)
    {
        this.IntervalTime = interval;
    }

    public void SetDamage(Stat dmg)
    {
        this.Dmg = dmg;
    }

    public void SetDuration(Stat duration)
    {
        this.Duration = duration;
    }

    public override void Play()
    {
        base.Play();
        isStart = true;
    }

    protected virtual void OnTriggerHit(Collider2D collider, ITarget target)
    {
        if (isStart == false) return;
        if (target != null)
        {   
            Impact(target as ActorBase);
        }
    }

    private async void Impact(ActorBase actor)
    {
        if (actor.GetCharacterType() != Caster.GetCharacterType())
        {
            var poison = (PoisonStatus)(await actor.StatusEngine.AddStatus(Caster, EStatus.Poison, source));
            if (poison != null)
            {
                poison.Init(Caster, actor);
                poison.SetCooldown(IntervalTime.Value);
                poison.SetDuration(Duration.Value);
                poison.SetDmgMul(Dmg.Value);
            }
        }
    }
    protected override  void OnDisable()
    {
        base.OnDisable();
        isStart = false;
    }

}