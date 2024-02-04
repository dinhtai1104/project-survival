using Cysharp.Threading.Tasks;
using Game.GameActor;
using Sirenix.OdinInspector;
using System;
using UnityEngine;

public class ActorSummonTask : SkillTask
{
    public string enemyId;
    public ActorBase enemyMini;
    [ShowInInspector]
    public LayerMask ignoreMask => 256;
    public override async UniTask Begin()
    {
        await base.Begin();
        SpawnMini();
    }

    protected virtual void SpawnMini()
    {
        Spawn(Caster.GetPosition());
        IsCompleted = true;
    }

    protected async virtual void Spawn(Vector3 pos)
    {
        var enemyhandler = GameController.Instance.GetEnemySpawnHandler();
        enemyMini = await enemyhandler.SpawnSingle(enemyId, (int)Caster.GetStatValue(StatKey.Level), Caster.GetPosition(), usePortal: false);
    }
}