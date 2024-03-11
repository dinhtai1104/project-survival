using Assets.Game.Scripts.Actor.States.Common;
using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "AISafeDistance.asset", menuName = SOUtility.GAME_AI + "AISafeDistance")]
public class AISafeDistance : BrainDecision
{
    public override bool Decide(Actor machine)
    {
        Actor actor = machine as Actor;
        var keepDistanceState = actor.Fsm.GetState<ActorKeepDistanceState>();

        if (keepDistanceState.IsCooldowning)
            return true;

        Actor enemy = keepDistanceState.FindPotentialThreat();
        keepDistanceState.CurrentThreat = enemy;

        if (enemy != null)
        {
            float safeDist = keepDistanceState.Distance;

            Vector3 diff = enemy.BotPosition - actor.BotPosition;
            float sqrDist = Vector3.SqrMagnitude(diff);

            if (sqrDist < safeDist * safeDist)
            {
                return false;
            }
        }
        return true;
    }
}

