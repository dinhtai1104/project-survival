using Cysharp.Threading.Tasks;
using Game.Effect;
using Game.GameActor;
using Game.Pool;
using UnityEngine;

[CreateAssetMenu(menuName = "CharacterBehaviours/ExplodeOnDead")]
public class ExplodeOnDead : CharacterBehaviour
{
    public ValueConfigSearch ExplodeRadius;
    public string effectId = "VFX_Rocket_Explosion";


    public override CharacterBehaviour SetUp(ActorBase character)
    {
        ExplodeOnDead instance = (ExplodeOnDead)base.SetUp(character);
        instance.effectId = effectId;
        instance.ExplodeRadius = ExplodeRadius;
        return instance;
    }
    public override void OnActive(ActorBase character)
    {
        active = true;
    }
    public override void OnDead(ActorBase character)
    {
        Explode(character);
    }
   

    Collider2D[] colliders = new Collider2D[10];
    void Explode(ActorBase character)
    {
        int count = Physics2D.OverlapCircleNonAlloc(character.GetMidTransform().position, ExplodeRadius.FloatValue, colliders, character.AttackHandler.targetMask);
        bool isExploded = count > 0;

        for (int i = 0; i < count; i++)
        {
            ITarget target = colliders[i].GetComponentInParent<ITarget>();

            if (target != null && (Object)target != character && character.AttackHandler.targetType.Contains(target.GetCharacterType()))
            {
                DamageSource damageSource = new DamageSource(character, (ActorBase)target, character.Stats.GetStat(StatKey.Dmg).Value, character);
                damageSource.posHit = character.GetMidTransform().position;
                damageSource._damageSource = EDamageSource.Effect;

                target.GetHit(damageSource, character);
                isExploded = true;
            }
            colliders[i] = null;
        }

        if (isExploded)
        {
            Vector3 pos = character.GetTransform().position;
            GameObjectSpawner.Instance.Get(effectId, res =>
            {
                res.GetComponent<EffectAbstract>().Active(pos);
            });
        }

    }
}
