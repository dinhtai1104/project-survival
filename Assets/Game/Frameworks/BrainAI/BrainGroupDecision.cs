using Game.GameActor;
using UnityEngine;

namespace Game.AI
{
    public enum BooleanOperator
    {
        AND,
        OR,
        NONE
    }
    [CreateAssetMenu(fileName = "AI_GroupDecision", menuName = "AI/AI_GroupDecision")]
    public class BrainGroupDecision : BrainDecision
    {
        [SerializeField] private DecisionOperator[] _decisionOperators;

        public override bool Decide(ActorBase actor)
        {
            var result = false;

            foreach (var decisionOperator in _decisionOperators)
            {
                var decision = decisionOperator.Decision.Decide(actor);
                decision = decisionOperator.IsNot ? !decision : decision;

                switch (decisionOperator.Operator)
                {
                    case BooleanOperator.NONE:
                        result = decision;
                        break;
                    case BooleanOperator.AND:
                        result = result && decision;
                        break;
                    default:
                        result = result || decision;
                        break;
                }
            }

            return result;
        }

        [System.Serializable]
        private class DecisionOperator
        {
            [SerializeField] private BrainDecision _decision;

            [SerializeField] private BooleanOperator _operator;

            [SerializeField] private bool _not;

            public BrainDecision Decision
            {
                get { return _decision; }
            }

            public BooleanOperator Operator
            {
                get { return _operator; }
            }

            public bool IsNot
            {
                get { return _not; }
            }
        }
    }
}
