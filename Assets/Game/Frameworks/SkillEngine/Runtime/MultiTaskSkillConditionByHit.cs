using Game.GameActor;
using Game.Skill;
using System;
using UnityEngine;

public class MultiTaskSkillConditionByHit : MultiTaskSkill
{
    public override bool CanCast
    {
        get
        {
            return base.CanCast && OnCheck();
        }
    }
    [SerializeField] private BoxCollider2D _hitCheck;
    [SerializeField] private LayerMask layerMask;
    private ContactFilter2D filter;
    private Collider2D[] colliders = new Collider2D[5];
    protected override void Start()
    {
        base.Start();
        filter = new ContactFilter2D
        {
            layerMask = layerMask,
            useTriggers = true,
            useLayerMask = true,
        };
    }

    private bool OnCheck()
    {
        colliders.CleanUp();
        var count = _hitCheck.OverlapCollider(filter, colliders);
        if (count == 0) return false;
        return true;
    }
}