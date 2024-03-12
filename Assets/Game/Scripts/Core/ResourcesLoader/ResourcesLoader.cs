using com.assets.loader.addressables;
using com.assets.loader.core;
using com.assets.loader.resources;
using Cysharp.Threading.Tasks;
using Pool;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.U2D;

public class ResourcesLoader
{
    private static ResourcesLoader instance;
    public static ResourcesLoader Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new ResourcesLoader();
                SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
            }
            return instance;
        }
    }

    private static void SceneManager_activeSceneChanged(Scene arg0, Scene arg1)
    {
        UnloadAll();
    }

    private static void UnloadAll()
    {
        Instance.objectLoader.ReleaseAll();
        Instance.objectResourceLoader.ReleaseAll();
        Instance.objectCached.Clear();
        Instance.assetCachedRequest.Clear();
        Instance.spriteAtlasCached.Clear();
        Instance.spriteCached.Clear();
    }

    private IAssetLoader objectLoader = new AddressableAssetLoader();
    private IAssetLoader objectResourceLoader = new ResourcesAssetLoader();

    private Dictionary<string, SpriteAtlas> spriteAtlasCached = new Dictionary<string, SpriteAtlas>();
    private Dictionary<string, Sprite> spriteCached = new Dictionary<string, Sprite>();
    private Dictionary<Type, Dictionary<string, object>> objectCached = new Dictionary<Type, Dictionary<string, object>>();
    private Dictionary<Type, Dictionary<string, AssetRequest>> assetCachedRequest = new Dictionary<Type, Dictionary<string, AssetRequest>>();

    private Dictionary<Type, Dictionary<string, object>> objectResourcesCached = new Dictionary<Type, Dictionary<string, object>>();
    private Dictionary<Type, Dictionary<string, AssetRequest>> assetResourceCachedRequest = new Dictionary<Type, Dictionary<string, AssetRequest>>();

    public static async UniTask<T> LoadAsync<T>(string path) where T : UnityEngine.Object
    {
        Logger.Log("Request: " + path);

        var type = typeof(T);
        if (!Instance.objectCached.ContainsKey(type))
        {
            Instance.objectCached.Add(type, new Dictionary<string, object>());
            Instance.assetCachedRequest.Add(type, new Dictionary<string, AssetRequest>());
        }
        var dic = Instance.objectCached[type];
        var request = Instance.assetCachedRequest[type];
        if (dic.ContainsKey(path))
        {
            return (T)dic[path];
        }
        else
        {
            var loader = Instance.objectLoader.LoadAsync<T>(path);
            
            await loader.Task;
            if (!dic.ContainsKey(path))
            {
                request.Add(path, loader);
                dic.Add(path, loader.Result);
            }
            return loader.Result;
        }
    }

    public static async UniTask<T> GetAsync<T>(string path, Transform parent)
    {
        var ins = await LoadAsync<GameObject>(path);
        if (ins == null) return default;
        var obj = PoolFactory.Spawn(ins, parent).GetComponent<T>();
        return (T)obj;
    }

    public static T Load<T>(string path) where T : UnityEngine.Object
    {
        var type = typeof(T);
        if (!Instance.objectCached.ContainsKey(type))
        {
            Instance.objectCached.Add(type, new Dictionary<string, object>());
            Instance.assetCachedRequest.Add(type, new Dictionary<string, AssetRequest>());
        }
        var dic = Instance.objectCached[type];
        var request = Instance.assetCachedRequest[type];
        if (dic.ContainsKey(path))
        {
            return (T)dic[path];
        }
        else
        {
            var loader = Instance.objectLoader.Load<T>(path);

            if (!dic.ContainsKey(path))
            {
                request.Add(path, loader);
                dic.Add(path, loader.Result);
            }
            return loader.Result;
        }
    }

    public static T Get<T>(string path, Transform parent)
    {
        var ins = Load<GameObject>(path);
        if (ins == null) return default;
        var obj = PoolFactory.Spawn(ins, parent).GetComponent<T>();
        return (T)obj;
    }
    public static async UniTask<GameObject> GetGOAsync(string path, Transform parent = null)
    {
        var ins = await LoadAsync<GameObject>(path);
        if (ins == null) return default;
        var obj = PoolFactory.Spawn(ins, parent);
        return (GameObject)obj;
    }
    

    public static Sprite GetSprite(string atlas, string nameSprite)
    {
        var path = atlas + "/" + nameSprite;
        if (Instance.spriteCached.ContainsKey(path))
        {
            return Instance.spriteCached[path];
        }
        var atlasArt = LoadAtlas(atlas);
        if (atlasArt != null)
        {
            var sprite = atlasArt.GetSprite(nameSprite);
            if (sprite != null)
            {
                Instance.spriteCached.Add(path, sprite);
                return sprite;
            }
            else
            {
                Debug.LogError("Not found sprite: " + nameSprite + " at " + atlas);
            }
        }
        else
        {
            Debug.LogError("Not found atlas: " + atlas);
        }
        return null;
    }
    public static SpriteAtlas LoadAtlas(string address)
    {
        var newAddress = "Atlas/" + address + ".spriteatlas";
        if (Instance.spriteAtlasCached.ContainsKey(newAddress))
        {
            return Instance.spriteAtlasCached[newAddress];
        }

        var atlasTask = Instance.objectLoader.Load<SpriteAtlas>(newAddress);
        var atlas = atlasTask.Result;
        if (atlas != null)
        {
            if (Instance.spriteAtlasCached.ContainsKey(newAddress))
            {
                return Instance.spriteAtlasCached[newAddress];
            }
            Instance.spriteAtlasCached.Add(newAddress, atlas);
            return Instance.spriteAtlasCached[newAddress];
        }
        else
        {
            return null;
        }
    }

    public static void UnloadAsset<T>(string name) where T : UnityEngine.Object
    {
        var type =typeof(T);
        if (!Instance.assetCachedRequest.ContainsKey(type)) return;
        var dict = Instance.assetCachedRequest[type];
        if (dict.ContainsKey(name))
        {
            Instance.objectLoader.Release(dict[name]);
            Instance.objectCached[type].Remove(name);
            dict.Remove(name);
        }
    }

    public static T LoadResource<T>(string path) where T: UnityEngine.Object
    {
        var type = typeof(T);
        if (!Instance.objectCached.ContainsKey(type))
        {
            Instance.objectCached.Add(type, new Dictionary<string, object>());
            Instance.assetCachedRequest.Add(type, new Dictionary<string, AssetRequest>());
        }
        var dic = Instance.objectCached[type];
        var request = Instance.assetCachedRequest[type];
        if (dic.ContainsKey(path))
        {
            return (T)dic[path];
        }
        else
        {
            var loader = Instance.objectResourceLoader.Load<T>(path);

            if (!dic.ContainsKey(path))
            {
                request.Add(path, loader);
                dic.Add(path, loader.Result);
            }
            return loader.Result;
        }
    }
}