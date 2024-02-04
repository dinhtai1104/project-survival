using Cysharp.Threading.Tasks;
using Game.GameActor;
using UnityEngine;

[CreateAssetMenu(menuName = "CharacterBehaviours/SpawnAllyOnDead")]
public class SpawnAllyOnDead : CharacterBehaviour
{
    public string spawnId;
    public bool dependHpOnParent = false;
    public ValueConfigSearch hpChild;
    public ValueConfigSearch total;
    public string effectId = "VFX_Enemy1_Split";


    public int spawnTimeSpacing = 100;
    public override CharacterBehaviour SetUp(ActorBase character)
    {
        SpawnAllyOnDead instance = (SpawnAllyOnDead)base.SetUp(character);
        instance.spawnId = spawnId;
        instance.total = total.Clone().SetId(character.gameObject.name);
        instance.hpChild = hpChild.Clone().SetId(character.gameObject.name);
        instance.dependHpOnParent = dependHpOnParent;
        instance.effectId = effectId;
        return instance;
    }
    public override void OnActive(ActorBase character)
    {
        active = true;
    }
    public override void OnDead(ActorBase character)
    {
        Debug.Log("ON DEAD");
        SpawnAlly(character).Forget();
    }
    async UniTaskVoid SpawnAlly(ActorBase character)
    {
        Messenger.Broadcast(EventKey.EnemyAllySpawn);
        var effect = (await Game.Pool.GameObjectSpawner.Instance.GetAsync(effectId)).GetComponent<Game.Effect.EffectAbstract>();
        effect.Active(character.GetMidTransform().position);
        int[] directions = { -1, 1, -1, 1 };
        for (int i = 0; i < total.IntValue; i++)
        {
            var spawn=await Game.Controller.Instance.gameController.GetEnemySpawnHandler().SpawnSingle(spawnId, (int)character.Stats.GetValue(StatKey.Level), character.GetPosition(),isStartBehaviourNow:false);

            if (dependHpOnParent)
            {
                var newHp = character.HealthHandler.GetMaxHP() * hpChild.FloatValue;
                character.Stats.SetBaseValue(StatKey.Hp, newHp);
                character.Stats.CalculateStats();
                character.HealthHandler.Reset(newHp, 0);
                Logger.Log("SET HP :" + hpChild.FloatValue + " " + character.HealthHandler.GetHealth() + "/" + character.HealthHandler.GetMaxHP());
            }


            try
            {
                Logger.Log("GET: " + (spawn.BehaviourTree.GetVariable("Direction") == null));
                if (spawn.BehaviourTree.GetVariable("Direction") == null)
                {
                    spawn.MoveHandler.Move(Vector3.right * directions[i], 1);
                }
                else
                {
                    spawn.BehaviourTree.SetVariableValue("Direction", directions[i]);
                }
            }
            catch
            {
               
            }

            spawn.StartBehaviours();

            spawn.MoveHandler.Jump(new Vector2(Random.Range(0f, 1f) > 0.5f ? 1 : -1, 1), 2);

            await UniTask.Delay(spawnTimeSpacing, ignoreTimeScale: false);
        }
    }
}
