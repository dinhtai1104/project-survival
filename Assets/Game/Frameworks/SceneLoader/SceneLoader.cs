using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class SceneLoader
{
    public delegate UniTaskVoid OnScenePresented(SceneLoader loader);
    public OnScenePresented onScenePresented;
    public delegate UniTaskVoid OnSceneLoaded(SceneLoader loader);
    public OnSceneLoaded onSceneLoaded;
    public delegate UniTaskVoid OnLastSceneHidden(SceneLoader loader);
    public OnLastSceneHidden onLastSceneHidden;
    public delegate UniTaskVoid OnSceneLoading(SceneLoader loader,float progress);
    public OnSceneLoading onSceneLoading;

    UnityEngine.ResourceManagement.ResourceProviders.SceneInstance scene;
    public bool isSceneLoaded;
    float delayTime = 1.5f;
    public SceneLoader(string sceneId)
    {
        LoadScene(sceneId);
    }

    public async UniTaskVoid LoadScene(string sceneId)
    {
        Logger.Log("LOAD SCENE: " + sceneId);
        isSceneLoaded = false;
        var op = Addressables.LoadSceneAsync(sceneId, UnityEngine.SceneManagement.LoadSceneMode.Single, false);

        op.Completed += (handle =>
        {
            Logger.Log("SCENE LOADED");
            scene = handle.Result;
            onSceneLoaded?.Invoke(this);
            isSceneLoaded = true;
        });
        CaculateProgress().Forget();
        async UniTaskVoid CaculateProgress()
        {
            while (!op.IsDone)
            {
                onSceneLoading?.Invoke(this,op.PercentComplete / 2f);
                await UniTask.Yield();
            }
        }

        await UniTask.Delay(400);
        onLastSceneHidden?.Invoke(this);



    }

    public async UniTask ActiveScene()
    {
        await UniTask.WaitUntil(() => isSceneLoaded);
        float time = 0;
        while (time < delayTime)
        {
            onSceneLoading?.Invoke(this,0.5f + (time / delayTime) / 2f);
            time += Time.fixedDeltaTime;
            await UniTask.Yield(PlayerLoopTiming.FixedUpdate);
        }
        await scene.ActivateAsync();

        await UniTask.Delay(500, ignoreTimeScale: true);
        onScenePresented?.Invoke(this);
        isSceneLoaded = false;


        //destroy all
        onSceneLoaded = null;
        onSceneLoading = null;
        onScenePresented = null;
        onLastSceneHidden = null;
    }

}
