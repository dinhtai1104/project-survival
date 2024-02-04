using com.mec;
using Game.GameActor;
using System;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(AutoDestroyObject))]
[RequireComponent(typeof(InvokeHitByTrigger))]
public class Boss10Skill2BallObject : BulletSimpleDamageObject
{
    [SerializeField] private AutoDestroyObject _autoDestroyObject;

    private Stat speedStat;

    public Stat SpeedStat { get => speedStat; set => speedStat = value; }

    protected override void OnValidate()
    {
        base.OnValidate();
        _autoDestroyObject = GetComponent<AutoDestroyObject>();
    }

    public override void Play()
    {
        Movement.Speed = speedStat;
        base.Play();
        _hit.onTrigger += OnTrigger;
        _hit.SetIsFullTimeHit(false);
        _hit.SetMaxHit(1);

        _autoDestroyObject?.SetDuration(15);
        var listModi = new List<ModifierSource>() { new ModifierSource(speedStat) };
        Messenger.Broadcast(EventKey.PreFire, Caster, (BulletBase)null, listModi);
    }

    private void OnTrigger(Collider2D collider,ITarget target)
    {
        //var actor = collider.GetComponent<ActorBase>();
        if (target != null)
        {
            Impact(target);
        }
    }
    private void Impact(ITarget target)
    {
        if (target.GetCharacterType() != Caster.GetCharacterType())
        {
            var damageSource = new DamageSource
            {
                Attacker = Caster,
                Defender = (ActorBase)target,
                _damage = DmgStat
            };
            damageSource.posHit = transform.position;
            damageSource._damageSource = EDamageSource.Weapon;
            target.GetHit(damageSource, this);
        }
    }

}