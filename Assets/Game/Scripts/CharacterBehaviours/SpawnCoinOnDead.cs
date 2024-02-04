using Cysharp.Threading.Tasks;
using Game.GameActor;
using UnityEngine;

[CreateAssetMenu(menuName = "CharacterBehaviours/SpawnCoinOnDead")]
public class SpawnCoinOnDead : CharacterBehaviour
{
    public string SpawnId,EffectId="VFX_Coin";
    public RangeValue TotalSpawn;

    //ms
    public int spawnTimeSpacing = 100;
    public override CharacterBehaviour SetUp(ActorBase character)
    {
        SpawnCoinOnDead instance = (SpawnCoinOnDead)base.SetUp(character);
        instance.SpawnId = SpawnId;
        instance.TotalSpawn = TotalSpawn;
        instance.spawnTimeSpacing = spawnTimeSpacing;
        return instance;
    }
    public override void OnActive(ActorBase character)
    {
        active = true;
    }
    public override void OnDead(ActorBase character)
    {
        Game.Pool.GameObjectSpawner.Instance.Get(EffectId, obj =>
        {
            obj.GetComponent<Game.Effect.EffectAbstract>().Active(character.GetMidTransform().position);
        });

        Spawn(character).Forget();
    }
    async UniTaskVoid Spawn(ActorBase character)
    {
        int total = TotalSpawn.GetRandomInt();
        for (int i = 0; i < total; i++)
        {
            var spawn = await Game.Pool.GameObjectSpawner.Instance.GetAsync(SpawnId);
            spawn.transform.position = character.GetMidTransform().position;
            spawn.gameObject.SetActive(true);
            await UniTask.Delay(spawnTimeSpacing, ignoreTimeScale: false);
        }
    }
}
