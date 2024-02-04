using Cysharp.Threading.Tasks;
using Game.GameActor;
using UnityEngine;

[CreateAssetMenu(menuName = "CharacterBehaviours/SpawnHealthOrbOnDead")]
public class SpawnHealthOrbOnDead : CharacterBehaviour
{
    private const string HealthOrbID = "HealthOrb";
    public ValueConfigSearch HealRateMin,HealRateMax;

    protected float healRate;
    public override CharacterBehaviour SetUp(ActorBase character)
    {
        SpawnHealthOrbOnDead instance = (SpawnHealthOrbOnDead)base.SetUp(character);
        instance.healRate = UnityEngine.Random.Range(HealRateMin.FloatValue,HealRateMax.FloatValue);
        return instance;
    }
    public override void OnActive(ActorBase character)
    {
        active = true;
    }
    public override void OnDead(ActorBase character)
    {
        Spawn(character).Forget();
    }
    async UniTaskVoid Spawn(ActorBase character)
    {
        var orb = (await Game.Pool.GameObjectSpawner.Instance.GetAsync(HealthOrbID)).GetComponent<HealthOrb>();
        orb.SetUp(healRate,character.GetMidTransform().position);
     
    }
}
