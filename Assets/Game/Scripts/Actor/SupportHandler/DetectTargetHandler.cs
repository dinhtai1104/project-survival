using Game.GameActor;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
public class DetectTargetHandler : MonoBehaviour, IBatchUpdate, IDetectTargetHandler
{
    public delegate void OnTargetFound(ActorBase character, ITarget target);
    public OnTargetFound onTargetFound;
    public bool directTargetMainActor=true;
    //place to spawn the health bar
    [ShowInInspector]
    private List<ITarget> foundTargets = new List<ITarget>();
    public ITarget CurrentTarget { get => (foundTargets != null && foundTargets.Count > 0) ? foundTargets[0] : null; }

    ActorBase currentActor;
    private void OnEnable()
    {
        currentActor = GetComponent<ActorBase>();
        UpdateSlicer.Instance.RegisterSlicedUpdate(this);
    }
    void OnDisable()
    {
        UpdateSlicer.Instance.DeregisterSlicedUpdate(this);
    }
    public float detectRange = 100;
    public LayerMask targetMask, wallMask;
    public Game.GameActor.ECharacterType targetType;

    float time = 0;
    Collider2D[] colliders = new Collider2D[10];
    List<ITarget> targets = new List<ITarget>();
    List<ITarget> tempTargets = new List<ITarget>();
    List<ITarget> targetsThreat = new List<ITarget>();
    List<ITarget> targetsNonThreat = new List<ITarget>();
    bool IsWallBetween(Vector3 a, Vector3 b)
    {
        return (Physics2D.Raycast(a, b - a, Vector3.Distance(a, b), wallMask).collider != null);
    }
    List<ITarget> FindTarget(ActorBase character)
    {
        if (character == null || character.WeaponHandler.GetCurrentAttackPoint() == null) return null;

        targets.Clear();
        tempTargets.Clear();
        targetsThreat.Clear();
        targetsNonThreat.Clear();
        int count =0;
        ITarget overrideTarget = null;
        if (directTargetMainActor)
        {
            count = 1;
            overrideTarget = Game.Controller.Instance.gameController.GetMainActor();
            if (Vector3.Distance(character.GetMidTransform().position, overrideTarget.GetMidTransform().position) < detectRange)
            {
                count = 1;
            }
            else
            {
                count = 0;
            }
        }
        else
        {
            count = Physics2D.OverlapCircleNonAlloc(character.WeaponHandler.GetCurrentAttackPoint().position, detectRange, colliders, targetMask);
        }

        if (count == 0)
        {
            return targets;
        }
        else
        {
            for (int i = 0; i < count; i++)
            {
                ITarget target = overrideTarget ==null? colliders[i].GetComponentInParent<ITarget>():overrideTarget;

                if ((Object)target == character || !target.CanFocusOn() || target.IsDead() || (target != null && !targetType.Contains(target.GetCharacterType()))) continue;
                if ((target as ActorBase).Tagger.HasTag(ETag.Immune)) continue;
                if (!(target as ActorBase).IsActived)
                {
                    if ((target as ActorBase).Tagger.HasTag(ETag.Stun))
                    {
                        // Still Focus Target
                    }
                    else
                    {
                        continue;
                    }
                }

                if (IsWallBetween(target.GetMidTransform().position, character.GetMidTransform().position))
                {
                    tempTargets.Add(target);
                    continue;
                }

                if (target.IsThreat())
                    targetsThreat.Add(target);
                else
                    targetsNonThreat.Add(target);
            }
            Vector3 playerPosition = character.GetMidTransform().position;
            targetsThreat.Sort((a, b) =>
            {
                return Vector2.Distance(playerPosition, a.GetPosition()) < Vector2.Distance(playerPosition, b.GetPosition()) ? -1 : 1;
            });

            targetsNonThreat.Sort((a, b) =>
            {
                return Vector2.Distance(playerPosition, a.GetPosition()) < Vector2.Distance(playerPosition, b.GetPosition()) ? -1 : 1;
            });

            ITarget weakestTarget = null;
            float minHP = 999999;

            for (int i = 0; i < targetsThreat.Count; i++)
            {
                if (targetsThreat[i].GetHealthPoint() < minHP)
                {
                    minHP = targetsThreat[i].GetHealthPoint();
                    weakestTarget = targetsThreat[i];
                }
            }
            // if the weakest target has lower health than player and not very far from the player
            if (weakestTarget != null && weakestTarget.GetHealthPoint() < character.GetHealthPoint() && Vector2.Distance(playerPosition, weakestTarget.GetPosition()) < Vector2.Distance(playerPosition, targetsThreat[0].GetPosition()) + 5)
            {
                targetsThreat.Insert(0, weakestTarget);
            }
            targets.AddRange(targetsThreat);
            targets.AddRange(targetsNonThreat);

            if (targets.Count == 0)
            {
                targets.AddRange(tempTargets);
            }

            for (int i = 0; i < colliders.Length; i++)
            {
                colliders[i] = null;
            }
            return targets;
        }
    }
    bool triggered = false;
    public void Search()
    {
        if (Time.time - time > 0.5f || (CurrentTarget != null && CurrentTarget.GetHealthPoint() <= 0))
        {
            foundTargets = FindTarget(currentActor);
            time = Time.time;
            if (!triggered && CurrentTarget != null)
            {
                triggered = true;
                onTargetFound?.Invoke(currentActor, CurrentTarget);
            }
            else if (triggered && CurrentTarget == null)
            {
                triggered = false;
                onTargetFound?.Invoke(currentActor, null);

            }
        }

    }


    public ITarget GetTarget(int index)
    {
        if (foundTargets != null && foundTargets.Count > 0)
        {
            if (index < foundTargets.Count)
            {
                return foundTargets[index];
            }
            else
            {
                return foundTargets[foundTargets.Count - 1];
            }
        }
        return null;
    }
    public void BatchUpdate()
    {
        Search();
    }

    public void BatchFixedUpdate()
    {
    }
}
