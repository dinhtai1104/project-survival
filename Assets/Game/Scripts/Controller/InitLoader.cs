using Cysharp.Threading.Tasks;
using Game.GameActor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InitLoader : UnityEngine.MonoBehaviour
{
    [SerializeField]
    private int targetFPS=60;
    // Start is called before the first frame update
    async UniTaskVoid Start()
    {
        if (!PlayerPrefs.HasKey("HDR"))
        {
#if UNITY_ANDROID
            PlayerPrefs.SetInt("HDR", 0);
#elif UNITY_IOS || UNITY_EDITOR
            PlayerPrefs.SetInt("HDR", 1);
#endif
        }

        UnityEngine.InputSystem.EnhancedTouch.EnhancedTouchSupport.Enable();
        Application.targetFrameRate = targetFPS;
        SceneLoader sceneLoader = new SceneLoader(SceneKey.LOADING);

        AsyncOperationHandle<UnityEngine.ResourceManagement.ResourceProviders.SceneInstance> op;
        //loading progress
        sceneLoader.onSceneLoading += async (loader,progress) =>
        {
        };
        //new scene has been shown
        sceneLoader.onScenePresented += async (loader) =>
        {
        };
        //on scene loaded
        sceneLoader.onSceneLoaded += async (loader) => 
        {
            //wait for splash scene end
            await UniTask.Delay(2500);
            await sceneLoader.ActiveScene();
        };
        //last scene has been hidden, but not removed
        sceneLoader.onLastSceneHidden += async (loader) => { };



        //DamageSource source = new DamageSource(null, null, 100);
        //Logger.Log(source.Value);
        //source.AddModifier(new StatModifier(EStatMod.Percent, 0.9f));
        //source.AddModifier(new StatModifier(EStatMod.Percent, 0.7f));
        //source.AddModifier(new StatModifier(EStatMod.Percent, 0.6f));
        //source.AddModifier(new StatModifier(EStatMod.Percent, 0.5f));
        //source.AddModifier(new StatModifier(EStatMod.Percent, 0.7f));
        //Logger.Log(source.Value);
    }

}
