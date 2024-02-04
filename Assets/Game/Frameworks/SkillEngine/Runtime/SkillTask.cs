using com.mec;
using Cysharp.Threading.Tasks;
using Game.GameActor;
using Game.Skill;
using Game.Tasks;
using UnityEngine;

public class SkillTask : Task, IStopHandler
{
    private BaseSkill _skill;
    public BaseSkill Skill => _skill;
    public ActorBase Caster { get; set; }
    public override void UnityAwake()
    {
        base.UnityAwake();
        Caster = GetComponentInParent<ActorBase>();
        if (Caster == null)
        {
            Caster = GetComponentInParent<ISkillEngine>().Owner;
        }
        if (_skill == null)
            _skill = GetComponentInParent<BaseSkill>();
    }
    public override async UniTask Begin()
    {
        if (Caster == null)
        {
            Caster = GetComponentInParent<ISkillEngine>().Owner;
        }
        await base.Begin();
    }
    public override async UniTask End()
    {
        //if (Skill)
        //{
        //    Skill.StartCooldown();
        //}
        Caster.Stats.RemoveModifiersFromSource(this);
        await base.End();
    }

    public virtual void OnStop()
    {
        IsCompleted = true;
        Caster.Stats.RemoveModifiersFromSource(this);
        Timing.KillCoroutines(gameObject);
    }

    public float GetAngleToTarget(UnityEngine.Transform pos)
    {
        var target = Caster.FindClosetTarget();
        if (target == null) return 0;
        var angle = GameUtility.GameUtility.GetAngle(pos, target.GetMidTransform());

        return angle;
    }

    public Vector2 GetPosTarget()
    {
        var target = Caster.FindClosetTarget();
        if (target == null) return UnityEngine.Vector2.zero;
        return target.GetMidPos();
    }
}
//[System.Serializable]
//public class SequenceTask 
//{
//    private BaseSkill _skill;
//    public BaseSkill Skill => _skill;
//    public ActorBase Caster { get; set; }
//    public override void UnityAwake()
//    {
//        base.UnityAwake();
//        Caster = GetComponentInParent<ActorBase>();
//        if (_skill == null)
//            _skill = GetComponentInParent<BaseSkill>();
//    }
//    public bool IsRunning { set; get; }
//    public bool ForceInterruptTask { set; get; }

//    public bool IsCompleted { set; get; } = false;

//    public virtual void UnityEnable() { }
//    public virtual void UnityDisable() { }
//    public virtual void UnityAwake() { }
//    public virtual void UnityStart() { }

//    public async void BeginEvent()
//    {
//        await Begin();
//    }

//    public virtual async UniTask Begin()
//    {
//        IsCompleted = false;
//        IsRunning = true;
//        await UniTask.Yield();
//    }

//    public virtual async UniTask End()
//    {
//        IsCompleted = true;
//        IsRunning = false;
//        await UniTask.Yield();
//    }

//    public virtual void Run()
//    {
//        if (IsCompleted || !IsRunning) return;
//        if (_completeWhenAllChildrenStop)
//        {
//            bool childrenComplete = true;
//            foreach (var parallelTask in _parallelTasks)
//            {
//                if (parallelTask.GetInstanceID() != GetInstanceID() && !parallelTask.IsCompleted)
//                {
//                    childrenComplete = false;
//                    break;
//                }
//            }

//            if (childrenComplete)
//                IsCompleted = true;
//        }
//    }
//}