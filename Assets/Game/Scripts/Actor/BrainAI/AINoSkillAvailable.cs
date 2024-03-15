using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BrainAI
{
    [CreateAssetMenu(fileName = "AINoSkillAvailable.asset", menuName = SOUtility.GAME_AI + "AINoSkillAvailable")]
    public class AINoSkillAvailable : BrainDecision
    {
        public override bool Decide(ActorBase actor)
        {
            return !actor.SkillCaster.HasAvailableSkill && !actor.SkillCaster.IsBusy;
        }
    }
}
