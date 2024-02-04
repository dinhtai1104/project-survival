using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum ENotice
{
    YesNo,
    OnlyNo,
    OnlyYes,
}

public class UINoticePanel : UI.Panel
{
    [SerializeField] private TextMeshProUGUI desciptionTxt;
    [SerializeField] private TextMeshProUGUI titleTxt;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private LayoutGroup layoutGroup;
    [SerializeField] private Button cancelButton, confirmButton, closeButton;
    private System.Action confirmCallback, cancelCallback;

    private void OnValidate()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }
    public override void PostInit()
    {
        canvasGroup.blocksRaycasts = true;
        cancelButton.onClick.AddListener(CancelOnClicked);
        confirmButton.onClick.AddListener(ConfirmOnClicked);
    }
    public void SetText(string notice, System.Action confirmCallback = null, System.Action cancelCallback = null, ENotice noticeType = ENotice.YesNo)
    {
        desciptionTxt.text = notice;
        this.cancelCallback = cancelCallback;
        this.confirmCallback = confirmCallback;

        switch (noticeType)
        {
            case ENotice.YesNo:
                break;
            case ENotice.OnlyNo:
                closeButton.gameObject.SetActive(false);
                confirmButton.gameObject.SetActive(false);
                break;
            case ENotice.OnlyYes:
                closeButton.gameObject.SetActive(false);
                cancelButton.gameObject.SetActive(false);
                break;
        }
    }

    public void SetConfirmCallback(System.Action callback)
    {
        this.confirmCallback = callback;

        FirebaseAnalysticController.Tracker.NewEvent("button_click")
            .AddStringParam("category", "notice")
            .AddStringParam("name", desciptionTxt.text)
            .Track();
    }
    public void SetCancelCallback(System.Action callback)
    {
        this.cancelCallback = callback;
    }

    public override void Close()
    {
        base.Close();
        cancelButton.onClick.RemoveListener(CancelOnClicked);
        confirmButton.onClick.RemoveListener(ConfirmOnClicked);
    }

    private void ConfirmOnClicked()
    {
        canvasGroup.blocksRaycasts = false;
        confirmCallback?.Invoke();
        Close();
    }
    private void CancelOnClicked()
    {
        canvasGroup.blocksRaycasts = false;
        cancelCallback?.Invoke();
        Close();
    }

    public void SetTitle(string title)
    {
        titleTxt.text = title;
    }
}
