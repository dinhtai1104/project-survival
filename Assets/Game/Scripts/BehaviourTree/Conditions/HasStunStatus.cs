using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class HasStunStatus : HasConditional
{
    [SerializeField] private SharedActor Actor;
    [SerializeField] private EStatus status;
    public override TaskStatus OnUpdate()
    {
        bool stun = Actor.Value.StatusEngine.HasStatus<StunStatus>();
        if ((!stun || !Actor.Value.Tagger.HasTag(ETag.Stun)) && (Actor.Value.IsActived))
        {
            return TaskStatus.Success;
        }
        return base.OnUpdate();
    }
}