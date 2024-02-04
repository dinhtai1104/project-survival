using Cysharp.Threading.Tasks;
using Game.GameActor;
using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRushSkillTask : SkillTask
{
    public ValueConfigSearch rushVelocity;
    public ValueConfigSearch rushReflectBeforeToTarget;

    private List<Vector3> listDirectionReflect = new List<Vector3>();
    public LayerMask groundReflectMask;
    private int currentReflect = 0;
    private Vector3 currentDir;
    private Vector3 trackTargetPos;
    public override async UniTask Begin()
    {
        currentReflect = 0;
        rushVelocity = rushVelocity.SetId(Caster.gameObject.name);
        rushReflectBeforeToTarget = rushReflectBeforeToTarget.SetId(Caster.gameObject.name);

        var target = Caster.FindClosetTarget();
        if (target == null)
        {
            IsCompleted = true;
            return;
        }
        await base.Begin();
        Caster.Stats.AddModifier(StatKey.SpeedMove, new StatModifier(EStatMod.Flat, rushVelocity.FloatValue), this);
      //  PrepareReflectDir();
        var dir = target.GetMidPos() - Caster.GetMidPos();
        trackTargetPos = target.GetMidPos();
        currentDir = dir;
        currentDir = currentDir.normalized;
    }

    private float timeRush = 0;
    public float timeRushMax = 3;
    public override void Run()
    {
        base.Run();
        // Chasing
        if (rushReflectBeforeToTarget.IntValue > 0)
        {
            if (currentReflect >= rushReflectBeforeToTarget.IntValue)
            {
                IsCompleted = true;
                return;
            }

            var hit = Physics2D.Raycast(Caster.GetMidPos(), currentDir, 1f, groundReflectMask);
            if (hit.collider != null)
            {
                currentDir = Vector3.Reflect(currentDir, hit.normal);
                currentDir.Normalize();
                currentReflect++;
            }
            else
            {
                //IsCompleted = true;
                //return;
            }

            if (currentReflect >= rushReflectBeforeToTarget.IntValue)
            {
                IsCompleted = true;
                return;
            }

            Caster.MoveHandler.Move(currentDir, 1);
        }
        else
        {
            if (GameUtility.GameUtility.GetRange(Caster.GetMidPos(), trackTargetPos) > 1f)
            {
                timeRush += Time.deltaTime;
                if (timeRush > timeRushMax)
                {
                    timeRush = 0;
                    IsCompleted = true;
                }
                //var target = Caster.FindClosetTarget();
                //var dir = target.GetMidPos() - Caster.GetMidPos();
                //trackTargetPos = target.GetMidPos();
                //currentDir = dir;

                Caster.MoveHandler.Move(currentDir, 1);
            }
            else
            {
                IsCompleted = true;
            }
        }

        Debug.DrawLine(Caster.GetMidPos(), Caster.GetMidPos() + currentDir * 1, Color.red);
        Caster.SetFacing(Caster.FindClosetTarget() as ActorBase);
    }

    private void PrepareReflectDir()
    {
        var target = Caster.FindClosetTarget();
        var dir = target.GetMidPos() - Caster.GetMidPos();

        for (int i = 0; i < rushReflectBeforeToTarget.IntValue; i += 1)
        {
            var hit = Physics2D.Raycast(Caster.GetMidPos(), dir, Mathf.Infinity, groundReflectMask);
            if (hit.collider != null)
            {
                var point = hit.point;
                dir = Vector3.Reflect(dir, hit.normal);
                listDirectionReflect.Add(dir);
            }
        }
    }
}