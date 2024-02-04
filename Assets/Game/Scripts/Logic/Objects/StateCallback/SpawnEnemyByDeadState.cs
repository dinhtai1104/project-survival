using Game.GameActor;
using Game.Level;
using Game.Pool;
using UnityEngine;

public class SpawnEnemyByDeadState : MonoBehaviour, IStateEnterCallback
{
    public bool DependHpOnParent = true;
    private ActorBase actor;
    public string idEnemy;
    public ValueConfigSearch hpChild;
    public ValueConfigSearch numberChild;
    public string VFX = "VFX_Boss_3_1_Death";
    public async void Action()
    {
        Logger.Log("SPAWN ENEMY "+ numberChild.IntValue);
        Messenger.Broadcast(EventKey.EnemyAllySpawn);
        var spawner = GameController.Instance.GetEnemySpawnHandler();
        if (spawner == null) return;
        Logger.Log("=>SPAWN ENEMY " + numberChild.IntValue);

        GameObjectSpawner.Instance.Get(VFX, t=>
        {
            var ef = t.GetComponent<Game.Effect.EffectAbstract>();
            ef.Active(actor.GetMidTransform().position, actor.transform.localScale.x);
        });

        //Logger.Log("HP: " + actor.Stats.GetValue(StatKey.Hp));
        int direct = 1;
        for (int i = 0; i < numberChild.IntValue; i++)
        {
            var character = await spawner.SpawnSingle(idEnemy, (int)actor.Stats.GetValue(StatKey.Level), actor.GetMidTransform().position + Random.onUnitSphere * 0.5f, isStartBehaviourNow: true);
            if (DependHpOnParent)
            {
                var newHp = actor.Stats.GetValue(StatKey.Hp) * hpChild.FloatValue;
                character.Stats.SetBaseValue(StatKey.Hp, newHp);
                character.Stats.CalculateStats();
                character.HealthHandler.Reset(newHp, 0);
                //Logger.Log("SET HP :" + hpChild.FloatValue+" "+character.HealthHandler.GetHealth() + "/" + character.HealthHandler.GetMaxHP());
            }
            character.MoveHandler.Move(Vector3.right * direct, 1);
            direct *= -1;
        }
    }

    public void SetActor(ActorBase actor)
    {
        this.actor = actor;
    }
}