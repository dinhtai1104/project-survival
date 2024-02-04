using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class FindAvailableDestinationAction : Action
{
    public Vector3 legitZoneSize;
    public SharedVector2 destination;
    [SerializeField]
    private LayerMask groundMask;
    [SerializeField]
    private bool followTarget = true;

    public SharedTransform target;

    public SharedActor Actor;
    Collider2D[] colliders=new Collider2D[1];
    Vector2 final = Vector2.zero ;
    Vector3 zero = Vector3.zero;
    public override TaskStatus OnUpdate()
    {
        int attemp = 10;
        do
        {
            final = followTarget?(target.Value==null?zero:target.Value.position):(Actor.Value.GetPosition());
            final.x += Random.Range(-legitZoneSize.x / 2f, legitZoneSize.x / 2);
            final.y += Random.Range(-legitZoneSize.y / 2f, legitZoneSize.y / 2);

            attemp--;
        } while ((final.y>11f ||Physics2D.OverlapCircleNonAlloc(final,2f,colliders,groundMask)>0 )&& attemp>0);
        destination.Value = final;


        return TaskStatus.Success;
    }
}