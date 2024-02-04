
using Game.GameActor;
using System;
using UnityEngine;

[CreateAssetMenu(menuName = "CharacterBehaviours/CopyTargetStat")]
public class CopyTargetStat : CharacterBehaviour
{
    public ECharacterType targetType;
    public StatKey targetStat;

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
                    if (actor.GetCharacterType() == targetType)
                    {
                        target = actor;
                        break;
                    }
                }
            }
            return target;
        }
    }
    ActorBase character;
    public override CharacterBehaviour SetUp(ActorBase character)
    {
        CopyTargetStat instance = (CopyTargetStat)base.SetUp(character);
        instance.targetType = targetType;
        instance.targetStat = targetStat;
        instance.character = character;


        return instance;
    }
    public override void OnActive(ActorBase character)
    {
        active = true;

        Target.Stats.AddListener(targetStat, OnStatUpdated);
    }
    public override void OnDeactive(ActorBase character)
    {
        base.OnDeactive(character);
        Target.Stats.RemoveListener(targetStat, OnStatUpdated);
    }
    private void OnStatUpdated(float value)
    {
        //Debug.Log($"TARGET UPDATE: {Target.gameObject.name} {value}");
        //Debug.Log($"=>> BEFORE STAT: {character.gameObject.name} {character.Stats.GetValue(targetStat, 0)}");
        character.Stats.SetBaseValue(targetStat, value);
        character.Stats.CalculateStats();
        //Debug.Log($"=>> NEW STAT: {character.gameObject.name} {character.Stats.GetValue(targetStat, 0)}");


    }

    public override void OnUpdate(ActorBase character)
    { 
    }

 
}
