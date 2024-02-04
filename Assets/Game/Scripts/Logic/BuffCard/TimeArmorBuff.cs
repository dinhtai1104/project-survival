using Game.GameActor.Buff;
using System;
using System.Collections.Generic;
using UnityEngine;

public class TimeArmorBuff : AbstractBuff
{
    public ValueConfigSearch timeArmor_RangeAffect = new ValueConfigSearch("Buff_TimeArmor_RangeAffect");
    public CircleCollider2D circleCollider2D;
    public float Radius;
    private StatModifier modifier = new StatModifier(EStatMod.PercentAdd, 0);
    private List<IMove> bulletsInRange = new List<IMove>();
    private List<BulletBase> bulletsInRange2 = new List<BulletBase>();
    private float time = 0.2f;
    private float timeCtr = 0.2f;
    public ContactFilter2D contactFilter;
    Collider2D[] colliderResult = new Collider2D[10];

    public override void Play()
    {
        colliderResult.CleanUp();

        transform.position = Caster.GetMidTransform().position;
        modifier.Value = GetValue(StatKey.SpeedMove);
        //circleCollider2D.transform.localScale = Vector3.one * timeArmor_RangeAffect.FloatValue;
        Radius = timeArmor_RangeAffect.FloatValue;
    }

    public override void OnUpdate(float dt)
    {
        base.OnUpdate(dt);
        if (timeCtr >= time)
        {
            timeCtr = 0;
            Scan();
        }
        timeCtr += dt;
    }
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(Caster.GetMidTransform().position, Radius);
    }
#endif
    List<IMove> moveCurrent = new List<IMove>();
    private void Scan()
    {
        int count = Physics2D.OverlapCircleNonAlloc(Caster.GetMidTransform().position, Radius, colliderResult,contactFilter.layerMask);
        //circleCollider2D.OverlapCollider(contactFilter, colliderResult);
        moveCurrent.Clear();

        // Add Collider Enter
        for(int i=0;i<count;i++)
        {
            var collider = colliderResult[i];
            if (collider != null)
            {
                var move = collider.GetComponent<IMove>();
                if (move != null)
                {
                    moveCurrent.Add(move);
                    if (!bulletsInRange.Contains(move))
                    {
                        if (!move.Speed.HasModifier(modifier))
                        {
                            move.Speed.AddModifier(modifier);
                        }
                        bulletsInRange.Add(move);
                    }
                }
                //var bullet = collider.GetComponent<BulletBase>();
                //if (bullet != null)
                //{
                //    bullet.Velocity = GetValue(StatKey.SpeedMove);
                //}
            }
        }

        // Remove Collider Exit

        for (int i = bulletsInRange.Count - 1; i >= 0; i--)
        {
            var moveCheck = bulletsInRange[i];
            if (!moveCurrent.Contains(moveCheck) || !moveCheck.IsActive)
            {
                moveCheck.Speed.RemoveModifier(modifier);
                bulletsInRange.RemoveAt(i);
            }
        }
    }
}
