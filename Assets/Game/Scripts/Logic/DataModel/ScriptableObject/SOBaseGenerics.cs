using Sirenix.OdinInspector;
using UnityEngine;

public class SOBaseGenerics<TData> : ScriptableObject where TData : class
{
    [ShowInInspector]
    public TData Data;
}