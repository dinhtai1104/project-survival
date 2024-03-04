using com.assets.loader.addressables;
using com.assets.loader.core;
using com.assets.loader.resources;
using Cysharp.Threading.Tasks;
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
        ResourcesLoader.Instance.UnloadAll();
    }

    private void UnloadAll()
    {
        objectLoader.ReleaseAll();
        objectCached.Clear();
        assetCachedRequest.Clear();
        spriteAtlasCached.Clear();
        spriteCached.Clear();
    }

    private IAssetLoader objectLoader = new AddressableAssetLoader();
    private IAssetLoader objectResourceLoader = new ResourcesAssetLoader();

    private Dictionary<string, SpriteAtlas> spriteAtlasCached = new Dictionary<string, SpriteAtlas>();
    private Dictionary<string, Sprite> spriteCached = new Dictionary<string, Sprite>();
    private Dictionary<Type, Dictionary<string, object>> objectCached = new Dictionary<Type, Dictionary<string, object>>();
    private Dictionary<Type, Dictionary<string, AssetRequest>> assetCachedRequest = new Dictionary<Type, Dictionary<string, AssetRequest>>();

    private Dictionary<Type, Dictionary<string, object>> objectResourcesCached = new Dictionary<Type, Dictionary<string, object>>();
    private Dictionary<Type, Dictionary<string, AssetRequest>> assetResourceCachedRequest = new Dictionary<Type, Dictionary<string, AssetRequest>>();

    public async UniTask<T> LoadAsync<T>(string path) where T : UnityEngine.Object
    {
        var type = typeof(T);
        if (!objectCached.ContainsKey(type))
        {
            objectCached.Add(type, new Dictionary<string, object>());
            assetCachedRequest.Add(type, new Dictionary<string, AssetRequest>());
        }
        var dic = objectCached[type];
        var request = assetCachedRequest[type];
        if (dic.ContainsKey(path))
        {
            return (T)dic[path];
        }
        else
        {
            var loader = objectLoader.LoadAsync<T>(path);
            
            await loader.Task;
            if (!dic.ContainsKey(path))
            {
                request.Add(path, loader);
                dic.Add(path, loader.Result);
            }
            return loader.Result;
        }
    }

    public async UniTask<T> GetAsync<T>(string path, Transform parent)
    {
        var ins = await LoadAsync<GameObject>(path);
        if (ins == null) return default;
        var obj = PoolManager.Instance.Spawn(ins, parent).GetComponent<T>();
        return (T)obj;
    }

    public T Load<T>(string path) where T : UnityEngine.Object
    {
        var type = typeof(T);
        if (!objectCached.ContainsKey(type))
        {
            objectCached.Add(type, new Dictionary<string, object>());
            assetCachedRequest.Add(type, new Dictionary<string, AssetRequest>());
        }
        var dic = objectCached[type];
        var request = assetCachedRequest[type];
        if (dic.ContainsKey(path))
        {
            return (T)dic[path];
        }
        else
        {
            var loader = objectLoader.Load<T>(path);

            if (!dic.ContainsKey(path))
            {
                request.Add(path, loader);
                dic.Add(path, loader.Result);
            }
            return loader.Result;
        }
    }

    public T Get<T>(string path, Transform parent)
    {
        var ins = Load<GameObject>(path);
        if (ins == null) return default;
        var obj = PoolManager.Instance.Spawn(ins, parent).GetComponent<T>();
        return (T)obj;
    }
    public async UniTask<GameObject> GetGOAsync(string path, Transform parent = null)
    {
        var ins = await LoadAsync<GameObject>(path);
        if (ins == null) return default;
        var obj = PoolManager.Instance.Spawn(ins, parent);
        return (GameObject)obj;
    }
    

    public Sprite GetSprite(string atlas, string nameSprite)
    {
        var path = atlas + "/" + nameSprite;
        if (spriteCached.ContainsKey(path))
        {
            return spriteCached[path];
        }
        var atlasArt = LoadAtlas(atlas);
        if (atlasArt != null)
        {
            var sprite = atlasArt.GetSprite(nameSprite);
            if (sprite != null)
            {
                spriteCached.Add(path, sprite);
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
    public SpriteAtlas LoadAtlas(string address)
    {
        var newAddress = "Atlas/" + address + ".spriteatlas";
        if (spriteAtlasCached.ContainsKey(newAddress))
        {
            return spriteAtlasCached[newAddress];
        }

        var atlasTask = objectLoader.Load<SpriteAtlas>(newAddress);
        var atlas = atlasTask.Result;
        if (atlas != null)
        {
            if (spriteAtlasCached.ContainsKey(newAddress))
            {
                return spriteAtlasCached[newAddress];
            }
            spriteAtlasCached.Add(newAddress, atlas);
            return spriteAtlasCached[newAddress];
        }
        else
        {
            return null;
        }
    }

    public void UnloadAsset<T>(string name) where T : UnityEngine.Object
    {
        var type =typeof(T);
        if (!assetCachedRequest.ContainsKey(type)) return;
        var dict = assetCachedRequest[type];
        if (dict.ContainsKey(name))
        {
            objectLoader.Release(dict[name]);
            objectCached[type].Remove(name);
            dict.Remove(name);
        }
    }

    public T LoadResource<T>(string path) where T: UnityEngine.Object
    {
        var type = typeof(T);
        if (!objectCached.ContainsKey(type))
        {
            objectCached.Add(type, new Dictionary<string, object>());
            assetCachedRequest.Add(type, new Dictionary<string, AssetRequest>());
        }
        var dic = objectCached[type];
        var request = assetCachedRequest[type];
        if (dic.ContainsKey(path))
        {
            return (T)dic[path];
        }
        else
        {
            var loader = objectResourceLoader.Load<T>(path);

            if (!dic.ContainsKey(path))
            {
                request.Add(path, loader);
                dic.Add(path, loader.Result);
            }
            return loader.Result;
        }
    }
}