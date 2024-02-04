using Game.GameActor;
using UnityEngine;

[CreateAssetMenu(menuName = "CharacterBehaviours/AttackTargetWhenInRange")]
public class AttackTargetWhenInRange : CharacterBehaviour
{
    public override CharacterBehaviour SetUp(ActorBase character)
    {
        AttackTargetWhenInRange instance = (AttackTargetWhenInRange)base.SetUp(character);
      
        return instance;
    }
    public override void OnActive(ActorBase character)
    {
        active = true;
    }
    float time = 0;
    float triggerTime = 0;
    bool foundTarget = false;
    public override void OnUpdate(ActorBase character)
    {
        if (!active) return;
        ITarget target = null;
        if (Time.time - time > 0.2f)
        {
            target = character.Sensor.CurrentTarget;
          

            if (target == null )
            {
                foundTarget = false;
                character.AttackHandler.Stop();
            }
            else
            {
                character.AttackHandler.Active();

            }
            time = Time.time;
        }


    }
}
