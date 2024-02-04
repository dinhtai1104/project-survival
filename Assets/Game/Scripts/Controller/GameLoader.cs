using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UI;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class GameVersionChecker 
{
    public static bool IsLatestVersion = true;
}

public class GameLoader : MonoBehaviour
{
    [SerializeField]
    private TMPro.TextMeshProUGUI signInText;
    CancellationTokenSource cancellation;

    // Start is called before the first frame update
    async UniTaskVoid Start()
    {
        await UniTask.WaitUntil(() => Sound.Controller.Instance.IsReady(),cancellationToken:cancellation.Token);
        await UniTask.WaitUntil(() => Application.genuineCheckAvailable && Application.genuine, cancellationToken: cancellation.Token);

       

        Sound.Controller.Instance.soundData.PlayMenuTheme();

        float time = Time.time;
        await UniTask.WaitUntil(() => FirebaseAuthentication.Instance.isReady ||Time.time-time>3, cancellationToken: cancellation.Token);
        try
        {
            await CheckNewVersionGame();
        }catch(System.Exception e)
        {
            Logger.LogError(e);
        }

        //login as guest
        await HandleLogin();

        InitSDK();
        if(FirebaseAuthentication.Instance.isReady && FirebaseAuthentication.Instance.User != null)
        {
            DataManager.Save.User.Id = FirebaseAuthentication.Instance.User.UserId;
        }
        if (!DataManager.Save.General.IsGameTutFinished)
        {
            Game.Controller.Instance.StartLevel(GameMode.Tutorial,-1);
            return;
        }


        //
        LoadingScreen loadingScreen = (await Addressables.InstantiateAsync("LoadingScreen")).GetComponent<LoadingScreen>();
        loadingScreen.Show();


        SceneLoader sceneLoader = new SceneLoader(SceneKey.MENU);

        //loading progress
        sceneLoader.onSceneLoading += async (loader,progress) =>
        {
            loadingScreen.SetProgress(progress);
        };
        //new scene has been shown
        sceneLoader.onScenePresented += async (loader) =>
        {
            loadingScreen.Hide();
        };
        //on scene loaded
        sceneLoader.onSceneLoaded += OnSceneLoaded;
        //last scene has been hidden, but not removed
        sceneLoader.onLastSceneHidden += async (loader) => { };
    }

    private async UniTask CheckNewVersionGame()
    {
        float time = Time.time;
        await UniTask.WaitUntil(() => RemoteConfigHandler.Instance.isReady||Time.time-time>3);
        int latestVersion = RemoteConfigHandler.Instance.GetValue("LatestVersion").TryGetInt();
        string platform = null;
#if UNITY_ANDROID
        platform = "AndroidBundleVersionSO";
#elif UNITY_IOS
        platform = "iOSBundleVersionSO";
#endif
        var currentVersion = (await Resources.LoadAsync(platform).ToUniTask()) as BundleVersionSO;


        Logger.Log("Old Version: " + currentVersion.BundleVersion + " ---- " + "New Version: " + latestVersion);
        while (latestVersion>(currentVersion.BundleVersion))
        {
            // Found new version =>== FORCE Update
            var uiNotice = await PanelManager.ShowNotice(I2Localize.GetLocalize("Notice/Notice.ForceUpdate"), noticeType: ENotice.OnlyYes);
            uiNotice.SetConfirmCallback(OnForceUpdate);
            uiNotice.SetTitle(I2Localize.GetLocalize("Notice/Notice.Title_ForceUpdate"));
            await UniTask.WaitUntil(() => uiNotice == null);
        }

        void OnForceUpdate()
        {
            OpenStorePage _page = new OpenStorePage("6474717017", Application.identifier);
            _page.OpenPage();
        }
    }

    async UniTask HandleLogin()
    {
        var panel = await UI.PanelManager.CreateAsync<SignInPanel>(AddressableName.UISignInPanel);
        panel.Show();
        panel.SetText("Signing In...");
        var user = await FirebaseAuthentication.Instance.SignInAsGuest();
        panel.SetText(user != null ? $"Welcome {(user.IsAnonymous ? "Player" : user.DisplayName)} " : "NULL USER");
        panel.SetID(user != null ? user.UserId : "null");

        if (user != null && user.IsAnonymous)
        {

            panel.ShowBtn();
            await UniTask.WaitUntil(() => panel == null || !panel.gameObject.activeSelf, cancellationToken: cancellation.Token);
            await UniTask.Delay(200, cancellationToken: cancellation.Token);
        }
        else
        {
            await UniTask.Delay(1000, cancellationToken: cancellation.Token);
        }
        await CloudSave.Controller.Instance.ValidateAndSave();
        


    }
    void InitSDK()
    {
        //always have AD
        AD.Controller.Instance.Init(true);
    }
    private void OnEnable()
    {
        cancellation = new CancellationTokenSource();
    }
    private void OnDestroy()
    {
        if (cancellation != null)
        {
            cancellation.Cancel();
            cancellation.Dispose();
            cancellation = null;
        }
    }
    async UniTaskVoid OnSceneLoaded(SceneLoader loader)
    {

        await loader.ActiveScene();

    }

}
