using Game.GameActor;
using UnityEngine;

namespace Game.AI
{
    [CreateAssetMenu(fileName = "AITimeSkillAvailable", menuName = "AI/AITimeSkillAvailable")]
    public class AITimeSkillAvailable : BrainDecision
    {
        [SerializeField]
        private float timer = 5;
        public override bool Decide(ActorBase actor)
        {
            Debug.Log((Time.time - actor.AttackHandler.lastSuccessAttack) +  ">"+timer);
            return Time.time-actor.AttackHandler.lastSuccessAttack>=timer;
        }
    }
}