using Cysharp.Threading.Tasks;
using Game.GameActor;
using UnityEngine;

public class ActorSpawnAllyTask : SkillTask
{
    public string allyId;
    public ValueConfigSearch total;
    public string effectId = "VFX_Enemy1_Split";
    public override async UniTask Begin()
    {
        await base.Begin();
        await SpawnAlly(Caster);
        IsCompleted = true;

    }
    async UniTask SpawnAlly(ActorBase character)
    {
        Messenger.Broadcast(EventKey.EnemyAllySpawn);
       
        var effect = (await Game.Pool.GameObjectSpawner.Instance.GetAsync(effectId)).GetComponent<Game.Effect.EffectAbstract>();
        effect.Active(character.GetMidTransform().position);
        for (int i = 0; i < total.IntValue; i++)
        {
            var spawn = await Game.Controller.Instance.gameController.GetEnemySpawnHandler().SpawnSingle(allyId, (int)character.Stats.GetValue(StatKey.Level), character.GetPosition(),2);
            Move(spawn, character.GetMidTransform().position + (Vector3)RotationToVector(90 + (i - total.IntValue / 2) * 60) * 4f).Forget();
        }
    }

    async UniTask Move(ActorBase actor,Vector3 position)
    {
        while (Vector3.Distance(actor.GetPosition(), position) > 1f)
        {
            actor.SetPosition(Vector3.Lerp(actor.GetPosition(), position, 0.2f));
            await UniTask.Yield();
        }
    }
    public static Vector2 RotationToVector(float rotationAngle)
    {
        return new Vector2(Mathf.Cos(rotationAngle * Mathf.Deg2Rad), Mathf.Sin(rotationAngle * Mathf.Deg2Rad));
    }
}
