using Game.AI;
using Game.Fsm;
using Game.GameActor;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class AIBrainInput : MonoBehaviour, IInputHandler
{
    [SerializeField]
    private MachineBrain machineBrain;
    public ActorBase Actor { set; get; }
    private IFsm machine;
    public void Initialize()
    {
        Actor = GetComponent<ActorBase>();
        machine = Actor.Machine;
    }

    public void Ticks()
    {
        var coreResult = false;
        // check core input
        if (machineBrain.CoreBrainTransition != null)
        {
            coreResult = CheckResultTransitions(machineBrain.CoreBrainTransition);
        }
        if (coreResult) return;

        // check neuron input
        foreach (var neuron in machineBrain.LocalBrainTransition)
        {
            if (!machine.IsCurrentState(neuron.StateType)) continue;
            CheckTransitions(neuron.Transitions);
            break;
        }

    }

    private bool CheckResultTransitions(IEnumerable<BrainTransition> transitions)
    {
        foreach (var transition in transitions)
        {
            bool result = transition.Decision.Decide(Actor);

            if (result)
            {
                if (transition.TrueBranchState == null) continue;
                if (!machine.IsCurrentState(transition.TrueBranchState))
                    machine.ChangeState(transition.TrueBranchState);
                return true;
            }
            else
            {
                if (transition.FalseBranchState == null) continue;
                if (!machine.IsCurrentState(transition.FalseBranchState))
                    machine.ChangeState(transition.FalseBranchState);
                return false;
            }
        }

        return false;
    }

    private void CheckTransitions(IEnumerable<BrainTransition> transitions)
    {
        foreach (var transition in transitions)
        {
            bool result = transition.Decision.Decide(Actor);

            if (result)
            {
                if (transition.TrueBranchState != null && !machine.IsCurrentState(transition.TrueBranchState))
                {
                    // It will set Next State Available not set CurrentState
                    machine.ChangeState(transition.TrueBranchState);
                }
            }
            else
            {
                if (transition.FalseBranchState != null && !machine.IsCurrentState(transition.FalseBranchState))
                {
                    machine.ChangeState(transition.FalseBranchState);
                }
            }
        }
    }

    public void SetActive(bool active)
    {
        throw new NotImplementedException();
    }
}

/*

AI được xử lý mô tả phía dưới
- Mỗi neuron sẽ cho ra output là NextState so với State nguồn
- Mỗi decision trong từng transition của neuron sẽ đưa ra NextState so với State nguồn
    - Kiểm tra liên tục để đưa ra đc Next State so với State nguồn
 
*/
