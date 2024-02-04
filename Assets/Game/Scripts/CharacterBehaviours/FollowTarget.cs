
using Game.GameActor;
using System;
using UnityEngine;

[CreateAssetMenu(menuName = "CharacterBehaviours/FollowTarget")]
public class FollowTarget : CharacterBehaviour
{
    public ECharacterType targetType;

    public Vector3 offset=new Vector3(-2,2.5f);
    public float followSpeed = 0.3f;
    private ActorBase target;
    private ActorBase Target
    {
        get
        {
            if (target == null)
            {
                var actors = GameObject.FindObjectsOfType<ActorBase>();
                foreach (var actor in actors)
                {
                    if (actor.GetCharacterType() == targetType && actor.IsActived)
                    {
                        target = actor;
                        break;
                    }
                }
            }
            return target;
        }
    }
    public override CharacterBehaviour SetUp(ActorBase character)
    {
        FollowTarget instance = (FollowTarget)base.SetUp(character);
        instance.offset = this.offset;
        instance.targetType = targetType;
        instance.followSpeed = followSpeed;



        return instance;
    }
    public override void OnActive(ActorBase character)
    {
        active = true;

       
        Messenger.AddListener<ActorBase>(EventKey.ChangePlayer, OnChangePlayer);
    }
    public override void OnDeactive(ActorBase character)
    {
        base.OnDeactive(character);
        Messenger.RemoveListener<ActorBase>(EventKey.ChangePlayer, OnChangePlayer);

    }

    private void OnChangePlayer(ActorBase arg1)
    {
        target = arg1;
    }

    float turnTime;
    public override void OnUpdate(ActorBase character)
    {
        if (!active) return;
        Vector3 offset = this.offset;
        offset.x *= Target.GetLookDirection().x < 0 ? -1 : 1;
        character.MoveHandler.MoveToPosition(Target.GetPosition() + offset, followSpeed);


        //facing opposite direction 
        if (Time.time - turnTime > 0.1f && character.GetFacingDirection() * Target.GetFacingDirection() < 0 )
        {
            SetFacing((Character)character,Target.GetFacingDirection());
            turnTime = Time.time;

        }


    }

    void SetFacing(Character character,int direction)
    {
        character.SetLookDirection(0, 0);
        character.SetFacing(direction);
        character.SetLookDirection(direction, 0);
    }
 
}
