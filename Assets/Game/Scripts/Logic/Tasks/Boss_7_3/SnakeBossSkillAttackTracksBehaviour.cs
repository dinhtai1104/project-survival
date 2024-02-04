using Game.GameActor;
using UnityEngine;

[CreateAssetMenu]
public class SnakeBossSkillAttackTracksBehaviour : CharacterBehaviour
{
    [Header("SnakeBoss")]
    public ValueConfigSearch mConfig_MinBomb;
    public ValueConfigSearch mConfig_MaxBomb;
    public override void OnUpdate(ActorBase character)
    {
        base.OnUpdate(character);
    }
}