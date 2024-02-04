using BehaviorDesigner.Runtime.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Game.Scripts.BehaviourTree.Conditions
{
    public class HasSamePosXWithTarget : HasConditional
    {
        [SerializeField] private SharedActor Actor;
        public override TaskStatus OnUpdate()
        {
            var target = Actor.Value.FindClosetTarget();
            if (target == null)
            {
                return TaskStatus.Failure;
            }
            if (Mathf.Abs(target.GetMidPos().x - Actor.Value.GetMidPos().x) < 0.5f)
            {
                return TaskStatus.Success;
            }

            return base.OnUpdate();
        }
    }
}
