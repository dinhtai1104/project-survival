using com.mec;
using Game.GameActor;
using Game.Skill;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ObjectClearWhenClearGame))]
public class CharacterObjectBase : ObjectBase, IDamageDealer
{
    public bool Poolable = true;
    public delegate void OnBeforePlayEvent();
    public delegate void OnBeforeDestroyEvent(CharacterObjectBase character);
    public OnBeforePlayEvent onBeforePlay;
    public OnBeforeDestroyEvent onBeforeDestroy;

    protected List<IUpdate> listUpdateParallel = new List<IUpdate>();
    private ActorBase _caster;
    public ActorBase Caster
    {
        get => _caster;
        set => _caster = value;
    }
    public IMove NullMove = new NullMoveBullet();
    public IRotate NullRotate = new NullRotate();
    public IAnimationHandler NullAnimation = new NullAnimationHandler();
    public ISkillEngine NullSkill = new NullSkillEngine();

    protected IMove _movement;
    protected IRotate _rotate;
    protected IAnimationHandler _animation;
    protected ISkillEngine _skill;

    public IRotate Rotate => _rotate ?? NullRotate;
    public IMove Movement => _movement ?? NullMove;
    public IAnimationHandler Animation => _animation ?? NullAnimation;
    public ISkillEngine SkillEngine => _skill ?? NullSkill;

    private void Awake()
    {
        _rotate = GetComponent<IRotate>();
        _movement = GetComponent<IMove>();
        _animation = GetComponent<IAnimationHandler>();
        _skill = GetComponent<ISkillEngine>();
    }

    public void InitForce()
    {
        _rotate = GetComponent<IRotate>();
        _movement = GetComponent<IMove>();
        _animation = GetComponent<IAnimationHandler>();
        _skill = GetComponent<ISkillEngine>();

        SkillEngine.Initialize(Caster);
        listUpdateParallel = new List<IUpdate>(GetComponentsInChildren<IUpdate>());
    }

    public virtual void SetSize(float size)
    {
        transform.localScale = Vector3.one * size;
    }
    public virtual void SetSize(Vector2 size)
    {
        transform.localScale = size;
    }

    public virtual Vector3 GetDamagePosition()
    {
        return transform.position;
    }

    public Transform GetTransform()
    {
        return transform;
    }

    public virtual void SetCaster(ActorBase caster)
    {
        _caster = caster;
        InitForce();
    }

    public virtual void Play() 
    {
        Timing.KillCoroutines(gameObject);
        InitForce();
        onBeforePlay?.Invoke();
        Rotate.Play();
        Movement.Move();
        foreach (var i in listUpdateParallel)
        {
            i.OnInit();
        }
        Timing.RunCoroutine(_OnUpdate(), gameObject);
    }

    protected virtual IEnumerator<float> _OnUpdate()
    {
        while (true)
        {
            SkillEngine.Ticks();
            foreach (var i in listUpdateParallel)
            {
                i.OnUpdate();
            }
            yield return Timing.DeltaTime;
        }
    }

    protected virtual void OnDisable()
    {
        Stop();
        Timing.KillCoroutines(gameObject);
        onBeforeDestroy?.Invoke(this);
        onBeforeDestroy = null;
    }

    public virtual void Stop()
    {
        SkillEngine.InteruptCurrentSkill();
    }
}