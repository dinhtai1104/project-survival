using Cysharp.Threading.Tasks;
using Game.GameActor;
using Game.Tasks;
using System;

public class SummonTailResultTask : SkillTask
{
    public ActorSummonTask summonTask;
    public TaskRunner stunTask;
    public ActorBase Enemymini => summonTask.enemyMini;

    public override async UniTask Begin()
    {
        await base.Begin();
        Messenger.AddListener<ActorBase, ActorBase>(EventKey.KilledBy, OnDie);
        Messenger.AddListener<ActorBase>(EventKey.AutoDestroyActor, OnAutoDestroyActor);
    }

    public override void OnStop()
    {
        Messenger.RemoveListener<ActorBase>(EventKey.AutoDestroyActor, OnAutoDestroyActor);
        Messenger.RemoveListener<ActorBase, ActorBase>(EventKey.KilledBy, OnDie);

        summonTask.enemyMini = null;
        base.OnStop();
    }
    public override UniTask End()
    {
        Messenger.RemoveListener<ActorBase>(EventKey.AutoDestroyActor, OnAutoDestroyActor);
        Messenger.RemoveListener<ActorBase, ActorBase>(EventKey.KilledBy, OnDie);
        summonTask.enemyMini = null;
        return base.End();
    }

    private void OnDie(ActorBase attack, ActorBase die)
    {
        if (die == Enemymini)
        {
            if (IsCompleted) return;
            IsCompleted = true;
            stunTask.RunTask();
            stunTask.OnComplete = null;
            var pos = die.transform.position;
            Caster.transform.position = pos;
            stunTask.OnComplete += () =>
            {
            };
        }
    }

    private void OnAutoDestroyActor(ActorBase miniBoss)
    {
        if (IsCompleted) return;
        if (miniBoss == Enemymini)
        {
            IsCompleted = true;
            Caster.transform.position = miniBoss.transform.position;
        }
    }
}