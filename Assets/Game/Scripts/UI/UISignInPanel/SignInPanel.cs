using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SignInPanel : UI.Panel
{
    [SerializeField]
    private TMPro.TextMeshProUGUI titleText,idText;

    public GameObject accountBtn,playBtn,loadingIcon;
    public override void PostInit()
    {
        accountBtn.SetActive(false);
        playBtn.SetActive(false);
    }
    public void SetText(string text)
    {
        titleText.SetText(text);
    }
    public void SetID(string text)
    {
        idText.SetText(text);
    }
    public void ShowBtn()
    {
        loadingIcon.SetActive(false);
        accountBtn.SetActive(true);
        playBtn.SetActive(true);
    }
    public void OpenAccount()
    {
        UI.PanelManager.CreateAsync<AccountPanel>(AddressableName.UIAccountPanel).ContinueWith<AccountPanel>(panel =>
        {
            panel.SetUp(OnSignedIn,OnFailed);
        }).Forget();
    }

    void OnSignedIn()
    {
        var user = FirebaseAuthentication.Instance.User;
        SetText(user != null ? string.Format(I2Localize.GetLocalize("Common/Welcome"), (user.IsAnonymous ? "Player" : user.DisplayName)) : "NULL USER");

        Close();
    }
    void OnFailed()
    {

    }
}
