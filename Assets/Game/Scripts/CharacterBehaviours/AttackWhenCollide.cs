using Cysharp.Threading.Tasks;
using Game.GameActor;
using UnityEngine;

[CreateAssetMenu(menuName = "CharacterBehaviours/AttackWhenCollide")]
public class AttackWhenCollide : CharacterBehaviour
{
    public float delay = 0.2f;
    public Vector2 offScale;
    public float pushForce = 10;

    CollideAttackTrigger trigger;

    public override CharacterBehaviour SetUp(ActorBase character)
    {
        AttackWhenCollide instance = (AttackWhenCollide)base.SetUp(character);
        instance.delay = delay;
        instance.offScale = offScale;
        instance.pushForce = pushForce;
        Game.Pool.GameObjectSpawner.Instance.Get("CollideAttackTrigger", obj =>
        {
            instance.trigger = obj.GetComponent<CollideAttackTrigger>();
            instance.trigger.SetUp(character, offScale, pushForce, delay);
        });
        return instance;
    }
    public override void OnActive(ActorBase character)
    {
        active = true;
    }
    public override void OnDead(ActorBase character)
    {
        active = false;
        trigger.gameObject.SetActive(false);
    }
    public override void OnDeactive(ActorBase character)
    {
        base.OnDeactive(character);
        trigger.gameObject.SetActive(false);

    }

    float time = 0;
    public override void OnUpdate(ActorBase character)
    {
        return;
        //if (!active) return;
        //base.OnUpdate(character);
        //if (Time.time - time > delay)
        //{
        //    if (character.Sensor.CurrentTarget != null && Vector2.Distance(character.GetPosition(),character.Sensor.CurrentTarget.GetPosition())<distance)
        //    {
        //        Attack(character);
        //        time = Time.time;
        //    }
        //}
    }

    //protected virtual void Attack(ActorBase character)
    //{

    //    float damage = character.Stats.GetValue(StatKey.Dmg);
    //    DamageSource damageSource = new DamageSource(character, (ActorBase)character.Sensor.CurrentTarget, damage);

    //    var modifier = new ModifierSource(damage);

    //    damageSource.Value = modifier.Value == 0 ? damage : modifier.Value;

    //    character.Sensor.CurrentTarget.GetHit(damageSource, null);
    //    ActorBase target = (ActorBase)character.Sensor.CurrentTarget;
    //    target.MoveHandler.AddForce((target.GetMidTransform().position- character.GetMidTransform().position).normalized * pushForce);
    //}
}
