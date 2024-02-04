using Game.GameActor;
using UnityEngine;

[CreateAssetMenu(menuName = "CharacterBehaviours/AttackWhenCollideTurnBack")]
public class AttackWhenCollideTurnBack : AttackWhenCollide
{
    //protected override void Attack(ActorBase character)
    //{
    //    //base.Attack(character);
    //    TurnBack(character);
    //}

    void TurnBack(ActorBase Caster)
    {
        ITarget target = Caster.GetComponent<DetectTargetHandler>().CurrentTarget;
        if (target != null)
        {
            int direction = target.GetPosition().x < Caster.GetPosition().x ? 1 : -1;
            Vector2 force = new Vector2(direction * 1.2f, 0.15f);
            Caster.MoveHandler.Jump(force, 10);
        }
    }
}