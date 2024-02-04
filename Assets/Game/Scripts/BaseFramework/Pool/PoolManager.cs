using System;
using Lean.Pool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PoolManager : LiveSingleton<PoolManager>
{
    protected void Start()
    {
        SceneManager.activeSceneChanged += OnActiveSceneChanged;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        SceneManager.activeSceneChanged -= OnActiveSceneChanged;
    }

    private void OnActiveSceneChanged(Scene curScene, Scene nextScene)
    {
        DespawnAll();
    }

    public T Spawn<T>(T prefab) where T : Component
    {
        return LeanPool.Spawn(prefab);
    }

    public T Spawn<T>(T prefab, Transform parent) where T : Component
    {
        return LeanPool.Spawn(prefab, parent);
    }

    public GameObject Spawn(GameObject prefab)
    {
        return LeanPool.Spawn(prefab);
    }

    public GameObject Spawn(GameObject prefab, Transform parent)
    {
        return LeanPool.Spawn(prefab, parent);
    }

    public void Despawn(Component clone, float delay = 0)
    {
        LeanPool.Despawn(clone, delay);
    }

    public void Despawn(GameObject clone, float delay = 0)
    {
        LeanPool.Despawn(clone, delay);
    }

    public void Clear(GameObject clone)
    {
        LeanPool.CleanUpByPrefab(clone);
    }
    public void DespawnAll()
    {
        LeanPool.DespawnAll();
    }
}
