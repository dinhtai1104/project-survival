using Cysharp.Threading.Tasks;
using Game.GameActor;
using UnityEngine;

[CreateAssetMenu(menuName = "CharacterBehaviours/Enemy7003Patrol")]
public class Enemy7003Patrol : CharacterBehaviour
{
    [SerializeField]
    private float leftSide, rightSide;
    public override CharacterBehaviour SetUp(ActorBase character)
    {
        Enemy7003Patrol instance = (Enemy7003Patrol)base.SetUp(character);
        instance.leftSide = leftSide;
        instance.rightSide = rightSide;
        
        return instance;
    }
    public override void OnActive(ActorBase character)
    {
        active = true;
        Vector3 dir = Vector3.right;

        character.MoveHandler.Move(dir, 1);
    }

    public override void OnUpdate(ActorBase character)
    {
        base.OnUpdate(character);
        if (character.GetPosition().x > rightSide)
        {
            character.SetPosition(new Vector3(leftSide, character.GetPosition().y));
        }
    }
}
