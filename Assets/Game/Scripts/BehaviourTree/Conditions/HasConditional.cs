using BehaviorDesigner.Runtime.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

public enum CheckType
{
    Time,
    Forever
}

public class HasConditional : Conditional
{
    [SerializeField] protected CheckType check;
    [SerializeField, ShowIf(nameof(check), CheckType.Time)] protected int count = 1;

    private int c = 0;

    public override TaskStatus OnUpdate()
    {
        switch (check)
        {
            case CheckType.Time:
                c++;

                if (c >= count)
                {
                    c = 0;
                    return TaskStatus.Failure;
                }
                else return TaskStatus.Running;
            case CheckType.Forever:
                return TaskStatus.Running;
        }

        return TaskStatus.Failure;
    }
}