using Cysharp.Threading.Tasks;
using UnityEngine;

public class ActorRandomFlyInAreaItselfState : ActorRandomFlyState
{
    private bool isInit = false;
    public ValueConfigSearch areaItself, minDistance;
    private Vector3 centerPos;

    public override async void InitializeStateMachine()
    {
        base.InitializeStateMachine();

        await UniTask.Delay(200);
        if (!isInit)
        {
            isInit = true;
            var area = areaItself.SetId(Actor.gameObject.name).Vector2Value;
            legitZoneSize = (Vector3)area;
            centerPos = Actor.GetMidTransform().position;
        }
    }
    protected override void ScanNewPosition()
    {
        Vector3 position = Actor.GetPosition();
        int index = 0;
        do
        {
            _targetPos.Set(Random.Range(-legitZoneSize.x / 2f, legitZoneSize.x / 2), Random.Range(-legitZoneSize.y / 2f, legitZoneSize.y / 2));
            _targetPos += (Vector2)centerPos;
            index++;
        } while ((Physics2D.OverlapCircleNonAlloc(_targetPos, 2f, colliders, _layerMaskObstacles) > 0 || Vector3.Distance(position,_targetPos)<minDistance.FloatValue) && index < 100);
    }

    public Vector2 GetNewPosition()
    {
        var _targetPos = Vector2.zero;
        int index = 0;
        do
        {
            _targetPos.Set(Random.Range(-legitZoneSize.x / 2f, legitZoneSize.x / 2), Random.Range(-legitZoneSize.y / 2f, legitZoneSize.y / 2));
            _targetPos += (Vector2)centerPos;
            index++;
        } while (Physics2D.OverlapCircleNonAlloc(_targetPos, 2f, colliders, _layerMaskObstacles) > 0 && index < 100);

        return _targetPos;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube((Vector2)centerPos, legitZoneSize);
    }
}