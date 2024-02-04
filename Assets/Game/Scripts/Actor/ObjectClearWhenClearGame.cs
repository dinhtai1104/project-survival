using System;
using UnityEngine;
using UnityEngine.UIElements;

public class ObjectClearWhenClearGame : MonoBehaviour
{
    private void OnEnable()
    {
        Messenger.AddListener<bool>(EventKey.GameClear, OnGameClear);
    }
    private void OnDisable()
    {
        Messenger.RemoveListener<bool>(EventKey.GameClear, OnGameClear);
    }
    private void OnDestroy()
    {
        Messenger.RemoveListener<bool>(EventKey.GameClear, OnGameClear);
    }
    private void OnGameClear(bool clear)
    {
        var pos = transform.position;

        Game.Pool.GameObjectSpawner.Instance.Get("DisableEffect", obj =>
        {
            obj.GetComponent<Game.Effect.EffectAbstract>().Active(pos);
        });
        try
        {
            PoolManager.Instance.Despawn(gameObject);
        }
        catch (Exception e)
        {

        }
    }
}