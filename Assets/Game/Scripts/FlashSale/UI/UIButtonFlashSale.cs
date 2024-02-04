using Assets.Game.Scripts.Utilities;
using com.mec;
using Cysharp.Threading.Tasks;
using Foundation.Game.Time;
using System;
using System.Collections.Generic;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class UIButtonFlashSale : UIBaseButton
{
    public EFlashSale FlashSale;
    [SerializeField] private TextMeshProUGUI timeLeftTxt;
    [SerializeField] private TextMeshProUGUI titleTxt;
    [SerializeField] private Image iconFS;
    private FlashSaleSave save;

    protected override void OnEnable()
    {
        base.OnEnable();
        transform.parent.gameObject.SetActive(false);
        //Setup();
    }

    private void Setup()
    {
        Timing.KillCoroutines(gameObject);
        save = DataManager.Save.FlashSale.GetSave(FlashSale);
        if (save == null || save.IsEnd || !save.IsActive || save.IsClaimed)
        {
            transform.parent.gameObject.SetActive(false);
            return;
        }
        else
        {
            transform.parent.gameObject.SetActive(true);
        }
        iconFS.sprite = ResourcesLoader.Instance.GetSprite(AtlasName.FeatureButton, $"FS_{FlashSale}");
        titleTxt.text = I2Localize.GetLocalize($"FlashSale/Title_{FlashSale}");
        Timing.RunCoroutine(_Ticks(), gameObject);
    }

    private IEnumerator<float> _Ticks()
    {
        while (true)
        {
            var left = save.TimeEnd - UnbiasedTime.UtcNow;
            timeLeftTxt.text = left.ConvertTimeToString();
            if (left.TotalSeconds <= 0 || save.IsEnd || !save.IsActive || save.IsClaimed) break;

            yield return Timing.WaitForSeconds(1f);
        }
        save.Deactive();
        Setup();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        Timing.KillCoroutines(gameObject);
    }
    public override void Action()
    {
        PanelManager.CreateAsync<UIFlashSalePanel>(AddressableName.UIFlashSalePanel.AddParams(FlashSale)).ContinueWith(t =>
        {
            t.Show(FlashSale);
        }).Forget();
    }

    public void SetActive()
    {
        Setup();
    }

    public bool IsValid()
    {
        return save.IsActive;
    }
}
