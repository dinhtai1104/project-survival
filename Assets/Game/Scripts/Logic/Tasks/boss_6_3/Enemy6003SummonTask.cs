using Cysharp.Threading.Tasks;
using Game.GameActor;
using Game.Pool;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Enemy6003SummonTask : SkillTask
{
    [ShowInInspector]
    protected static List<ActorBase> summonedEnemies = new List<ActorBase>();
    protected Vector3 groundPos;
    protected Vector2 boundMap = new Vector2();

    public string _enemyId;
    public string _animationSkill;
    public string _vfxSummon = "VFX_6003_Summon";
    public ValueConfigSearch maxSummon;
    public ValueConfigSearch summonInSkill;
    public ValueConfigSearch distanceBtwTower;
    public List<ActorBase> summoned = new List<ActorBase>();
    public LayerMask groundMask;

    public override async UniTask Begin()
    {
        PrepareBoundMap();
        if (Caster.FindClosetTarget() == null || maxSummon.IntValue <= summoned.Count)
        {
            IsCompleted = true;
            return;
        }
        await base.Begin();

        Caster.AnimationHandler.SetAnimation(_animationSkill, false);
        Caster.AnimationHandler.onCompleteTracking += AnimationHandler_onCompleteTracking;
        Caster.AnimationHandler.onEventTracking += AnimationHandler_onEventTracking;
    }

    public override bool HasTask()
    {
        if (Caster.FindClosetTarget() == null || maxSummon.IntValue <= summoned.Count)
        {
            return false;
        }
        return true;
    }

    private void AnimationHandler_onEventTracking(Spine.TrackEntry trackEntry, Spine.Event e)
    {
        if (trackEntry.Animation.Name == _animationSkill)
        {
            if (e.Data.Name == "attack_tracking")
            {
                SpawnMiniBoss();
            }
        }
    }

    private async void SpawnMiniBoss()
    {
        var enemySpawner = GameController.Instance.GetEnemySpawnHandler();
        if (enemySpawner == null)
        {
            return;
        }

        var points = GetRandomPositionWithoutOverlapping(groundPos, boundMap, distanceBtwTower.FloatValue, summonInSkill.IntValue);
        for (int j = 0; j < summonInSkill.IntValue - summoned.Count; j++)
        {
            var pos = points[j];
            // Raycast ground;
            var raycast = Physics2D.Raycast(pos, Vector3.down, Mathf.Infinity, groundMask);
            if (raycast.collider != null)
            {
                pos = raycast.point;
            }

            GameObjectSpawner.Instance.Get(_vfxSummon, (t) =>
            {
                t.GetComponent<Game.Effect.EffectAbstract>().Active(pos);
            });

            var enemy = await enemySpawner.SpawnSingle(_enemyId, (int)Caster.GetStatValue(StatKey.Level), pos);
            enemy.SetActorSpawner(Caster);
            summoned.Add(enemy as ActorBase);
            summonedEnemies.Add(enemy as ActorBase);
            enemy.onSelfDie += OnDie;
        }
    }
    public static List<Vector3> GetRandomPositionWithoutOverlapping(Vector3 starting, Vector2 size,
       float distanceBetweenPosition, int positionNumber)
    {
        var points = new List<Vector3>();

        foreach (var i in summonedEnemies)
        {
            points.Add(new Vector3(i.transform.position.x, 0));
        }

        int countTry = 0;
        for (var i = 0; i < positionNumber;)
        {
            var randX = UnityEngine.Random.Range(size.x, size.y);
            var rd = new Vector2(randX, starting.y);
            var point = new Vector3(rd.x, rd.y, 0);
            countTry++;
            if (points.Count == 0)
            {
                points.Add(point);
                i++;
                continue;
            }
            for (var j = 0; j < points.Count; j++)
            {
                if ((point - points[j]).sqrMagnitude > distanceBetweenPosition * distanceBetweenPosition)
                {
                    if (j == points.Count - 1)
                    {
                        points.Add(point);
                        i++;
                        countTry = 0;
                    }

                    continue;
                }

                if (countTry > 10)
                {
                    points.Add(point);
                    i++;
                    countTry = 0;
                    break;
                }

                break;
            }
        }
        points.Add(points[points.Count - 1]);
        points.RemoveRange(0, summonedEnemies.Count);

        return points;
    }
    private void OnDie(ActorBase current)
    {
        if (summoned.Contains(current))
        {
            current.onSelfDie -= OnDie;
            summoned.Remove(current);
            summonedEnemies.Remove(current);
        }
    }

    public override void OnStop()
    {
        IsCompleted = true;
        Caster.AnimationHandler.onEventTracking -= AnimationHandler_onEventTracking;
        Caster.AnimationHandler.onCompleteTracking -= AnimationHandler_onCompleteTracking;
        base.OnStop();
    }
    public override UniTask End()
    {
        Caster.AnimationHandler.onEventTracking -= AnimationHandler_onEventTracking;
        Caster.AnimationHandler.onCompleteTracking -= AnimationHandler_onCompleteTracking;
        return base.End();
    }

    private void AnimationHandler_onCompleteTracking(Spine.TrackEntry trackEntry)
    {
        if (trackEntry.Animation.Name == _animationSkill)
        {
            IsCompleted = true;
        }
    }

    private void PrepareBoundMap()
    {
        var raycast = Physics2D.Raycast(Caster.GetPosition(), Vector3.down, Mathf.Infinity, groundMask);
        if (raycast.transform != null)
        {
            groundPos = raycast.point;
        }

        raycast = Physics2D.Raycast(groundPos + Vector3.up * 0.5f, Vector3.left, Mathf.Infinity, groundMask);
        if (raycast.transform != null)
        {
            boundMap.x = raycast.point.x + 0.5f;
        }

        raycast = Physics2D.Raycast(groundPos + Vector3.up * 0.5f, Vector3.right, Mathf.Infinity, groundMask);
        if (raycast.transform != null)
        {
            boundMap.y = raycast.point.x - 0.5f;
        }
    }

    private void OnDrawGizmos()
    {
        var gPos = new Vector3();
        if (Caster == null) return;
        var raycast = Physics2D.Raycast(Caster.GetPosition(), Vector3.down, Mathf.Infinity, groundMask);
        if (raycast.transform != null)
        {
            gPos = raycast.point;
        }
        Gizmos.DrawLine(Caster.GetPosition(), gPos);

        raycast = Physics2D.Raycast(gPos + Vector3.up * 0.5f, Vector3.left, Mathf.Infinity, groundMask);
        if (raycast.transform != null)
        {
            Gizmos.DrawLine(gPos + Vector3.up * 0.5f, raycast.point);
        }

        raycast = Physics2D.Raycast(gPos + Vector3.up * 0.5f, Vector3.right, Mathf.Infinity, groundMask);
        if (raycast.transform != null)
        {
            Gizmos.DrawLine(gPos + Vector3.up * 0.5f, raycast.point);
        }
    }
}