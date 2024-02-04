using com.mec;
using Game.Fsm;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

public class BaseSkillState : BaseState
{
    public ValueConfigSearch _valueCooldown = new ValueConfigSearch("", "2");
    [SerializeField] protected Stat _cooldown;

    public float Cooldown
    {
        set => _cooldown.BaseValue = value;
        get => _cooldown.Value;
    }
    private float _cooldownTimer;

    public int SkillId { set; get; }
    public bool IsCooldowning { private set; get; }
    public float CooldownTimer => Mathf.Clamp(_cooldownTimer, 0f, Cooldown);
    public bool DisableCooldown { protected set; get; }


    public override void InitializeStateMachine()
    {
        base.InitializeStateMachine();
        _valueCooldown.SetId(Actor.gameObject.name);
        _cooldown = new Stat(_valueCooldown.FloatValue);
    }

    public override void Enter()
    {
        base.Enter();

        if (!DisableCooldown && Cooldown > 0f)
        {
            StartCooldowning();
        }

    }
    protected void StartCooldowning()
    {
        _cooldownTimer = Cooldown;
        IsCooldowning = true;
        Timing.RunCoroutine(_Cooldowning(), gameObject);
    }

    private IEnumerator<float> _Cooldowning()
    {
        while (_cooldownTimer > 0f)
        {
            _cooldownTimer -= Time.deltaTime;
            yield return 0f;
        }

        IsCooldowning = false;
    }


    [Button]
    public void ResetCoolDown(float cooldownTime)
    {
        Cooldown = cooldownTime;
        _cooldownTimer = 0;
    }
}