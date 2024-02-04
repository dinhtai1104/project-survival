using Cysharp.Threading.Tasks;
using Game.GameActor;
using UnityEngine;

[CreateAssetMenu(menuName = "CharacterBehaviours/SpawnBossChestOnDead")]
public class SpawnBossChestOnDead : CharacterBehaviour
{
    public string EffectId="VFX_Coin";
    public override CharacterBehaviour SetUp(ActorBase character)
    {
        SpawnBossChestOnDead instance = (SpawnBossChestOnDead)base.SetUp(character);
        instance.EffectId = EffectId;
        return instance;
    }
    public override void OnActive(ActorBase character)
    {
        active = true;
    }
    public override void OnDead(ActorBase character)
    {
     

        HandleBossSpecialChest(character,((BattleGameController)Game.Controller.Instance.gameController).Stage+1).Forget();
    }
    async UniTask HandleBossSpecialChest(ActorBase character,int stage)
    {
        Game.Pool.GameObjectSpawner.Instance.Get(EffectId, obj =>
        {
            obj.GetComponent<Game.Effect.EffectAbstract>().Active(character.GetMidTransform().position);
        });

        switch (stage)
        {
            case 10:
            case 20:
                Game.Pool.GameObjectSpawner.Instance.Get("SilverBossChest", obj =>
                {
                    obj.transform.position = character.GetMidTransform().position;
                    obj.gameObject.SetActive(true);
                });
                break;

            case 30:
                Game.Pool.GameObjectSpawner.Instance.Get("GoldenBossChest", obj =>
                {
                    obj.transform.position = character.GetMidTransform().position;
                    obj.gameObject.SetActive(true);
                });
                break;
#if UNITY_EDITOR
            default:
                Game.Pool.GameObjectSpawner.Instance.Get("GoldenBossChest", obj =>
                {
                    obj.transform.position = character.GetMidTransform().position;
                    obj.gameObject.SetActive(true);
                });
                break;
#endif
        }
    }
}
