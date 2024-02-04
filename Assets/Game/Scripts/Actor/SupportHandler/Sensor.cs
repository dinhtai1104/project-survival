using Game.GameActor;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

public class Sensor  :MonoBehaviour
{
    //place to spawn the health bar
    [ShowInInspector]
    private List<ITarget> foundTargets = new List<ITarget>();

    public float detectRange = 100;
    [SerializeField]
    private LayerMask targetMask, wallMask;
    public Game.GameActor.ECharacterType targetType;

    Collider2D[] colliders = new Collider2D[10];
    List<ITarget> targets = new List<ITarget>();
    bool IsWallBetween(Vector3 a, Vector3 b)
    {
        return (Physics2D.Raycast(a, b - a, Vector3.Distance(a, b), wallMask).collider != null);
    }
    List<ITarget> FindTarget(ActorBase character, Vector3 position, List<ITarget> exclude)
    {
        if (character == null) return null;

        targets.Clear();

        int count = Physics2D.OverlapCircleNonAlloc(position, detectRange, colliders,targetMask);

        for(int i = 0; i < count; i++)
        {
            ITarget target = colliders[i].GetComponentInParent<ITarget>();
            if (target != null && (exclude==null || !exclude.Contains(target))&& !target.IsDead() && targetType.Contains(target.GetCharacterType()) && !IsWallBetween(position,target.GetMidTransform().position))
            {
                this.targets.Add(target);
            }
        }
        for(int i = 0; i < count; i++)
        {
            colliders[i] = null;
        }


        return targets;
    }
    public ITarget Search(Character caster,Vector3 position,List<ITarget> exclude=null)
    {
        targetMask = caster.Sensor.targetMask;
        wallMask = caster.Sensor.wallMask;
        targetType = caster.Sensor.targetType;

        foundTargets = FindTarget(caster,position,exclude);

        if (foundTargets.Count == 0) return null;
        if (exclude == null || exclude.Count == 0) return foundTargets[0];


        return foundTargets[0];


    }
    public List<ITarget> SearchAll(Character caster, Vector3 position, List<ITarget> exclude = null)
    {
        targetMask = caster.Sensor.targetMask;
        wallMask = caster.Sensor.wallMask;
        targetType = caster.Sensor.targetType;

        foundTargets = FindTarget(caster, position, exclude);
        return foundTargets;


    }
    public ITarget SearchManually(Character caster, Vector3 position, List<ITarget> exclude = null)
    {
        foundTargets = FindTarget(caster, position, exclude);

        if (foundTargets.Count == 0) return null;
        if (exclude == null || exclude.Count == 0) return foundTargets[0];


        return foundTargets[0];


    }


}
