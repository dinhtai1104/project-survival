using Game.GameActor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Game.Scripts.Logic.Tasks.Boss_8_3
{
    [CreateAssetMenu(menuName = "CharacterBehaviours/FloatMovementBehaviour")]
    public class FloatMovementBehaviour : CharacterBehaviour
    {
        public ValueConfigSearch offsetY;
        private Vector2 upper, lower;
        private Vector2 startOrigin;
        public override CharacterBehaviour SetUp(ActorBase character)
        {
            var bh = (FloatMovementBehaviour) base.SetUp(character);

            bh.offsetY = offsetY.Clone();

            return bh;
        }

        private bool isMoveUp = false;
        public override void OnActive(ActorBase character)
        {
            base.OnActive(character);
            active = true;
            upper = character.GetPosition() + offsetY.FloatValue * Vector3.up;
            lower = character.GetPosition() - offsetY.FloatValue * Vector3.up;
            startOrigin = character.GetPosition();
        }
        public override void OnUpdate(ActorBase character)
        {
            if (active == false) return;
            base.OnUpdate(character);

            var newPoint = character.GetPosition();
            if (isMoveUp)
            {
                newPoint = Vector3.MoveTowards(newPoint, upper, character.GetStatValue(StatKey.SpeedMove) * Time.deltaTime);
                if (Vector2.Distance(character.GetPosition(), upper) < 0.2f)
                {
                    isMoveUp = false;
                    return;
                }
            }
            else
            {
                newPoint = Vector3.MoveTowards(newPoint, lower, character.GetStatValue(StatKey.SpeedMove) * Time.deltaTime);
                if (Vector2.Distance(character.GetPosition(), lower) < 0.2f)
                {
                    isMoveUp = true;
                    return;
                }
            }
            character.SetPosition(newPoint);
        }
    }
}
