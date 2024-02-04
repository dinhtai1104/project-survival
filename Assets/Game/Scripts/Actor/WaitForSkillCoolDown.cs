using Game.GameActor;
using Game.Skill;

namespace InputController
{
    public class WaitForSkillCoolDown : Condition
    {
        public ActorBase Actor;

        public WaitForSkillCoolDown(ActorBase actor)
        {
            this.Actor = actor;
        }
        public override bool Check()
        {
            if (Actor.SkillEngine.IsBusy) return false;

            return true;

        }
    }
    public class WaitForSkillEmpty : Condition
    {
        public ActorBase Actor;

        public WaitForSkillEmpty(ActorBase actor)
        {
            this.Actor = actor;
        }
        public override bool Check()
        {
            var skill = Actor.SkillEngine.GetSkill(0) as MultiTaskSkill;
            int maxCast = skill.maxCastTime.IntValue;
            if ( (maxCast != 0 && skill.totalCast >= maxCast)) return true;

            return false;

        }
    }
}