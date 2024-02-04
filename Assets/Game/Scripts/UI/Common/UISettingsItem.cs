using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UISettingsItem : MonoBehaviour
{
    public ESettingButton settingButton = ESettingButton.Music;
    [SerializeField] private Button mainButton;
    [SerializeField] private TextMeshProUGUI stateTxt;
    private GeneralSave generalSave;

    public Sprite buttonRed;
    public Sprite buttonGreen;
    private void OnEnable()
    {
        Messenger.AddListener(EventKey.ChangeLanguage, UpdateLanguage);
        mainButton.onClick.AddListener(OnChangeClicked);
        Init();
    }

    private void OnDisable()
    {
        Messenger.RemoveListener(EventKey.ChangeLanguage, UpdateLanguage);
        mainButton.onClick.RemoveListener(OnChangeClicked);
    }

    private void UpdateLanguage()
    {
        Init();
    }

    private void Init()
    {
        generalSave = DataManager.Save.General;
        Setup();
    }

    private void Setup()
    {
        switch (settingButton)
        {
            case ESettingButton.Music:
                stateTxt.text = generalSave.IsOnMusic ? I2Localize.GetLocalize("Common/On") : I2Localize.GetLocalize("Common/Off");
                mainButton.image.sprite = generalSave.IsOnMusic ? buttonGreen : buttonRed;
                break;
            case ESettingButton.Sound:
                stateTxt.text = generalSave.IsOnSFX ? I2Localize.GetLocalize("Common/On") : I2Localize.GetLocalize("Common/Off");
                mainButton.image.sprite = generalSave.IsOnSFX ? buttonGreen : buttonRed;
                break;
            case ESettingButton.Vibrate:
                stateTxt.text = generalSave.IsOnVibrate ? I2Localize.GetLocalize("Common/On") : I2Localize.GetLocalize("Common/Off");
                mainButton.image.sprite = generalSave.IsOnVibrate ? buttonGreen : buttonRed;
                break;
            case ESettingButton.Graphic:
                stateTxt.text = generalSave.IsHDR ? I2Localize.GetLocalize("Common/On") : I2Localize.GetLocalize("Common/Off");
                mainButton.image.sprite = generalSave.IsHDR ? buttonGreen : buttonRed;
                break;
        }
    }

    private void OnChangeClicked()
    {
        switch (settingButton)
        {
            case ESettingButton.Music:
                ChangeMusic();
                break;
            case ESettingButton.Sound:
                ChangeSound();
                break;
            case ESettingButton.Vibrate:
                ChangeVibrate();
                break;
            case ESettingButton.Graphic:
                ChangeGraphic();
                break;
        }
    }

    private void ChangeMusic()
    {
        generalSave.IsOnMusic = !generalSave.IsOnMusic;
        //Sound.Controller.MusicEnable = generalSave.IsOnMusic;
        Sound.Controller.Instance.UpdateMusic();

        stateTxt.text = generalSave.IsOnMusic ? I2Localize.GetLocalize("Common/On") : I2Localize.GetLocalize("Common/Off");
        mainButton.image.sprite = generalSave.IsOnMusic ? buttonGreen : buttonRed;
    }

    private void ChangeSound()
    {
        generalSave.IsOnSFX = !generalSave.IsOnSFX;
        //Sound.Controller.SfxEnable= generalSave.IsOnSFX;
        stateTxt.text = generalSave.IsOnSFX ? I2Localize.GetLocalize("Common/On") : I2Localize.GetLocalize("Common/Off");
        mainButton.image.sprite = generalSave.IsOnSFX ? buttonGreen : buttonRed;
    }

    private void ChangeVibrate()
    {
        generalSave.IsOnVibrate = !generalSave.IsOnVibrate;
        stateTxt.text = generalSave.IsOnVibrate ? I2Localize.GetLocalize("Common/On") : I2Localize.GetLocalize("Common/Off");
        mainButton.image.sprite = generalSave.IsOnVibrate ? buttonGreen : buttonRed;
    }
    private void ChangeGraphic()
    {
        generalSave.IsHDR = !generalSave.IsHDR;
        stateTxt.text = generalSave.IsHDR ? I2Localize.GetLocalize("Common/On") : I2Localize.GetLocalize("Common/Off");
        mainButton.image.sprite = generalSave.IsHDR ? buttonGreen : buttonRed;
    }
}