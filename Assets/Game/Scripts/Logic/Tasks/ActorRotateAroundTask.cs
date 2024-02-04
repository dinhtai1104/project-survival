using Cysharp.Threading.Tasks;
using Game.Tasks;
using UnityEngine;

public class ActorRotateAroundTask : Task
{
    public enum RotateType
    {
        Z, 
        X,
        Y,
    }
    public RotateType type = RotateType.Z;
    public Transform Target;
    public float rotateSpeed;

    private void FixedUpdate()
    {
        if (IsCompleted) return;
        var x = type == RotateType.X ? rotateSpeed : 0;
        var y = type == RotateType.Y ? rotateSpeed : 0;
        var z = type == RotateType.Z ? rotateSpeed : 0;
        Target.Rotate(new Vector3(x, y, z) * Time.fixedDeltaTime);
    }

    public override async UniTask End()
    {
        await base.End();
        Target.rotation = Quaternion.identity;
    }
}