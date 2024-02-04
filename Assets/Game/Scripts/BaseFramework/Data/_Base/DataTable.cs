using BansheeGz.BGDatabase;
using Sirenix.OdinInspector;
using System.Collections.Generic;

[System.Serializable]
public abstract class DataTable
{
    public abstract void GetDatabase();
    public abstract void Clear();
}
[System.Serializable]
public abstract class DataTable<T, E> : DataTable
{
    [ShowInInspector]
    public Dictionary<T, E> Dictionary = new Dictionary<T, E>();
    public override void Clear()
    {
        Dictionary = new Dictionary<T, E>();
    }

    public E Get(T key) 
    { 
        if (Dictionary.ContainsKey(key)) return Dictionary[key];
        return (E)default;
    }
}