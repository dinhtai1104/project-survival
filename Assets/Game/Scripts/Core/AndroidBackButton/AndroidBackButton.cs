using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

public class AndroidBackButton
{
    private readonly struct Data
    {
        public Object Key { get; }
        public Func<bool> Callback { get; }

        public Data
        (
            Object key,
            Func<bool> callback
        )
        {
            Key = key;
            Callback = callback;
        }
    }

    private const int InitialCapacity = 8;

    private List<Data> list = new List<Data>(InitialCapacity);

    public bool IsDisable { get; set; }
    public int Count => list.Count;
    public Object[] Keys => list.Select(x => x.Key).ToArray();

    public Func<bool> CanClick { private get; set; }

    public event Action OnClick;

#if UNITY_EDITOR
    [UnityEditor.InitializeOnEnterPlayMode]
    private void RuntimeInitializeOnLoadMethod()
    {
        list?.Clear();
        list = new List<Data>(InitialCapacity);

        CanClick = default;
        OnClick = default;
    }
#endif

    public void Clear()
    {
        list.Clear();
    }

    public void Add(Object key, IAndroidBackButtonClickable clickable)
    {
        Add(key, clickable.AndroidBackClick);
    }

    public void Add(Object key, Action callback)
    {
        Add(key, () =>
        {
            callback();
            return true;
        });
    }

    public void Add(Object key, Func<bool> callback)
    {
        list.Add(new Data(key, callback));
    }

    public void Remove(Object key)
    {
        var item = list.Find(x => x.Key == key);
        list.Remove(item);
    }

    public void Update()
    {
        for (var i = list.Count - 1; 0 <= i; i--)
        {
            var x = list[i];
            if (x.Key != null) continue;
            list.Remove(x);
        }

        if (!Input.GetKeyDown(KeyCode.Escape)) return;

        if (IsDisable) return;
        if (list.Count <= 0) return;
        if (CanClick != null && !CanClick()) return;

        var count = list.Count;
        var lastIndex = count - 1;
        var data = list[lastIndex];
        var key = data.Key;
        var callback = data.Callback;

        if (callback == null) return;

        var result = callback();

        if (!result) return;

        OnClick?.Invoke();

        for (var i = 0; i < list.Count; i++)
        {
            var x = list[i];
            if (x.Key != key) continue;
            list.Remove(x);
            break;
        }
    }

    public override string ToString()
    {
        var result = string.Empty;
        foreach (var data in list)
        {
            result = result + data.Key.name + ",";
        }

        return result;
    }
}