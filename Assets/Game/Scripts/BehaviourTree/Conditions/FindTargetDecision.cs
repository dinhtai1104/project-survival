using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class FindTargetDecision : Conditional
{
    public string targetTag;
    public SharedTransform target;
    public override void OnAwake()
    {
        // Cache all of the transforms that have a tag of targetTag
         target.Value = UnityEngine.GameObject.FindGameObjectWithTag(targetTag).transform;
   
    }

    public override TaskStatus OnUpdate()
    {
        return TaskStatus.Success;
    }

   
}
