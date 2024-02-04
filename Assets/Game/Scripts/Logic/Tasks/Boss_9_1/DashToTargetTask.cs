using Cysharp.Threading.Tasks;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Game.Scripts.Logic.Tasks.Boss_9_1
{
    public class DashToTargetTask : SkillTask
    {
        public ValueConfigSearch rushTime;
        public ValueConfigSearch rushSpeed;

        private StatModifier speedModifier;
        private float time = 0;
        private Vector2 dir;
        public LayerMask stopMask;
        public override async UniTask Begin()
        {
            dir = Caster.GetFacingDirection() * Vector2.right;
            time = 0;
            rushTime.SetId(Caster.gameObject.name);
            rushSpeed.SetId(Caster.gameObject.name);
            await base.Begin();
            speedModifier = new StatModifier(EStatMod.FlatMul, rushSpeed.FloatValue);
            Caster.Stats.AddModifier(StatKey.SpeedMove, speedModifier, this);
        }

        public override void Run()
        {
            base.Run();
            if (time < rushTime.FloatValue)
            {
                if (Physics2D.CircleCast(Caster.GetMidPos(), Caster.transform.localScale.x * 0.5f, Caster.MoveHandler.lastMove, 2f, stopMask))
                {
                    IsCompleted = true;
                    return;
                }
                Caster.MoveHandler.Move(dir.normalized, 1);
                time += Time.deltaTime;
            }
            else
            {
                Caster.MoveHandler.Stop();
                IsCompleted = true;
            }
        }

        public override UniTask End()
        {
            Caster.Stats.RemoveModifier(StatKey.SpeedMove, speedModifier);
            return base.End();
        }
        public override void OnStop()
        {
            Caster.Stats.RemoveModifier(StatKey.SpeedMove, speedModifier);
            base.OnStop();
        }
    }
}
