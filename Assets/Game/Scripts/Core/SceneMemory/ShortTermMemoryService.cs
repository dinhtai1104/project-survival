using com.sparkle.core;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Using to store temporary data between two scenes.
/// </summary>
[Service(typeof(IShortTermMemoryService))]
public class ShortTermMemoryService : MonoBehaviour, IShortTermMemoryService
{
    private readonly Dictionary<Type, ISceneMemory> _memoryLookup;

    public bool Initialized { set; get; }

    public int Priority => 1;

    public ShortTermMemoryService()
    {
        _memoryLookup = new Dictionary<Type, ISceneMemory>();
    }

    public bool HasMemory<T>() where T : ISceneMemory
    {
        var type = typeof(T);
        return _memoryLookup.ContainsKey(type);
    }

    public bool CheckAndForget<T>() where T : ISceneMemory
    {
        var type = typeof(T);
        if (_memoryLookup.ContainsKey(type))
        {
            Forget<T>();
            return true;
        }
        return false;
    }

    public void Remember<T>(T memory) where T : ISceneMemory
    {
        var type = typeof(T);

        if (_memoryLookup.ContainsKey(type))
        {
#if UNITY_EDITOR
            Debug.Log("Overwrite memory of type " + type.Name);
#endif
            _memoryLookup[type] = memory;
            return;
        }

        _memoryLookup.Add(type, memory);
    }

    /// <summary>
    /// Remove data from memory. Need to call when scene exit to free up memory.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public bool Forget<T>() where T : ISceneMemory
    {
        var type = typeof(T);

        if (!_memoryLookup.ContainsKey(type))
        {
#if UNITY_EDITOR
            Debug.LogError("Memory type not found" + type.Name);
#endif
            return false;
        }

        _memoryLookup.Remove(type);
        return true;
    }

    public T RetrieveMemory<T>() where T : ISceneMemory
    {
        var type = typeof(T);

        if (_memoryLookup.ContainsKey(type)) return (T)_memoryLookup[type];

#if UNITY_EDITOR
        Debug.LogError("Memory type not found " + type.Name);
#endif
        return default(T);
    }

    public bool RetrieveMemory<T>(out T memory) where T : ISceneMemory
    {
        var type = typeof(T);
        memory = default;

        if (_memoryLookup.ContainsKey(type))
        {
            memory = (T)_memoryLookup[type];
            return true;
        }

#if UNITY_EDITOR
        Debug.LogError("Memory type not found " + type.Name);
#endif
        return false;
    }
}