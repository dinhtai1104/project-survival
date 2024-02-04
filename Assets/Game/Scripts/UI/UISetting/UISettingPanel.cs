using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISettingPanel : UI.Panel
{
    [SerializeField]
    private TMPro.TextMeshProUGUI userIdText,versionText;
    public void OpenAccount()
    {
        UI.PanelManager.CreateAsync<AccountPanel>(AddressableName.UIAccountPanel).ContinueWith<AccountPanel>(panel =>
        {
            panel.SetUp(OnSignedIn,OnFailed);
        }).Forget();
    }
    void OnSignedIn()
    {
       
    }
    void OnFailed()
    {

    }
    public void OpenSave()
    {
        if (FirebaseAuthentication.Instance.User != null && FirebaseAuthentication.Instance.User.IsAnonymous)
        {
            OpenAccount();
            return;
        }

        CloudSave.Controller.Instance.Validate().ContinueWith(status=> 
        {
            if (status == CloudSave.EStatus.Normal)
            {
                UI.PanelManager.CreateAsync<UICloudSavePanel>(AddressableName.UICloudSavePanel).ContinueWith(panel =>
                {
                    panel.SetUp(CloudSave.Controller.Instance.Local);
                }).Forget();

                Close();

            }
        }).Forget();
       
    }

    public void Logout()
    {
        FirebaseAuthentication.Instance.LogOut();
        UI.PanelManager.CreateAsync<UIMessagePanel>(AddressableName.UIMessagePanel).ContinueWith(panel =>
        {
            panel.SetUp("Logged out");
        }).Forget();
    }
    public void ShowGDPR()
    {
        GDPRHandler.ShowDialog();
    }
    public override void PostInit()
    {
        userIdText.SetText($"ID: {FirebaseAuthentication.Instance.User.UserId}");


        string platform = null;
#if UNITY_ANDROID
        platform = "AndroidBundleVersionSO";
#elif UNITY_IOS
        platform = "iOSBundleVersionSO";
#endif
        Resources.LoadAsync(platform).ToUniTask().ContinueWith(config => 
        {
            versionText.SetText($"V{((BundleVersionSO)config).BundleVersion}");
        }).Forget();

    }

}
