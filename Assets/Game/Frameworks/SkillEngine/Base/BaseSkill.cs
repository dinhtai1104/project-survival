using Game.GameActor;
using Game.Skill;
using System;
using UnityEngine;

public abstract class BaseSkill : MonoBehaviour, ISkill
{
    private ActorBase _caster;
    public int totalCast = 0;
    [SerializeField] private int _idSkill;
    [SerializeField] protected Stat _cooldown;
    private float _cooldownTimer = 0;
    private bool _isCooldowning = true;
    private bool isExecuting = false;
    public ActorBase Caster => _caster;
    public int Id => _idSkill;
    public virtual bool CanCast => !_isCooldowning && !IsExecuting;
    public bool IsCooldowning
    {
        get
        {
            if (_isCooldowning || IsExecuting)
                return false;
            return true;
        }
    }
    public float CoolDownTimer
    {
        get
        {
            return _cooldownTimer;
        }
    }
    public bool IsCoolingDown
    {
        get { return _isCooldowning; }
    }
    public bool IsExecuting
    {
        set
        {
            isExecuting = value;
        }
        get { return isExecuting; }
    }

    protected virtual void Start()
    {
    }

    public virtual void Initialize(ActorBase actor)
    {
        this._caster = actor;
        totalCast = 0;

       
        _cooldown.RecalculateValue();
        StartCooldown();

    }
    public virtual void Cast()
    {
        IsExecuting = true;
        OnCasting();
    }
    public virtual void Stop()
    {
        IsExecuting = false;
    }
    public void SetCoolDown(float time)
    {
        _cooldownTimer = time;

    }
    public float GetCoolDown()
    {
        return _cooldown.Value;
    }
    public virtual void StartCooldown()
    {
        _isCooldowning = true;
        SetCoolDown(0);
    }

    public void Ticks()
    {
        if (_isCooldowning)
        {
            _cooldownTimer += Time.deltaTime;
            if (_cooldownTimer >= _cooldown.Value)
            {
                _isCooldowning = false;
                _cooldownTimer = 0f;
                OnCooldownComplete();
            }
        }
        if (IsExecuting)
        {
            OnExecuting();
        }
    }

    protected virtual void OnCooldownComplete()
    {

    }

    protected virtual void OnCasting()
    {
    }


    protected virtual void OnExecuting()
    {
    }

    public void AddModifierCooldown(StatModifier modifier)
    {
        _cooldown.AddModifier(modifier);
    }

    public void RemoveModifierCooldown(StatModifier modifier)
    {
        if (_cooldown.HasModifier(modifier))
        {
            _cooldown.RemoveModifier(modifier);
        }
    }

    public Stat GetCooldownStat()
    {
        return _cooldown;
    }
}