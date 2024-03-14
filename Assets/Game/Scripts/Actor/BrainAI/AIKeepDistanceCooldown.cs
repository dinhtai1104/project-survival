using Engine;
using Engine.State.Common;
using Engine.State.Unit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "AI_KeepDistanceCooldown.asset", menuName = SOUtility.GAME_AI + "AI_KeepDistanceCooldown")]
public class AIKeepDistanceCooldown : BrainDecision
{
    public override bool Decide(Engine.Actor actor)
    {
        if (actor.Fsm.HasState<ActorKeepDistanceState>())
        {
            return actor.Fsm.GetState<ActorKeepDistanceState>().IsCooldowning;
        }
        return false;
    }
}
