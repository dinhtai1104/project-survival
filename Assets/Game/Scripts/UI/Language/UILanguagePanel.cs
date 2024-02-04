using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UILanguagePanel : MonoBehaviour
{
    [SerializeField] private Button prevBtn;
    [SerializeField] private Button nextBtn;
    [SerializeField] private TextMeshProUGUI languageTxt;
    private int currentLanguageId = 0;
    private List<string> languageList = new List<string>();
    private void OnEnable()
    {
        languageList = I2.Loc.LocalizationManager.GetAllLanguages(true);
        currentLanguageId = languageList.FindIndex(t=> t == I2.Loc.LocalizationManager.CurrentLanguage);

        languageTxt.text = I2.Loc.LocalizationManager.CurrentLanguage;

        prevBtn.onClick.AddListener(PrevButton);
        nextBtn.onClick.AddListener(NextButton);

        UpdateVisual();
    }
    private void OnDisable()
    {
        prevBtn.onClick.RemoveListener(PrevButton);
        nextBtn.onClick.RemoveListener(NextButton);
    }

    private void UpdateVisual()
{
        prevBtn.gameObject.SetActive(true);
        nextBtn.gameObject.SetActive(true);
        currentLanguageId = Mathf.Clamp(currentLanguageId, 0, languageList.Count);
        if (currentLanguageId <= 0)
        {
            prevBtn.gameObject.SetActive(false);
        }
        if (currentLanguageId >= languageList.Count - 1)
        {
            nextBtn.gameObject.SetActive(false);
        }
    }

    private void PrevButton()
    {
        var lastId = currentLanguageId;
        currentLanguageId--;
        UpdateVisual();
        UpdateLanguage(currentLanguageId);
    }

    private async void UpdateLanguage(int currentLanguageId)
    {
        var language = languageList[currentLanguageId];

        var code = I2.Loc.LocalizationManager.GetLanguageCode(language);
        I2.Loc.LocalizationManager.SetLanguageAndCode(language, code);
        Debug.Log("Update language: " + language);
        await GameSceneManager.Instance.UpdateTmpFontAsset(language);
        Messenger.Broadcast(EventKey.ChangeLanguage);

        languageTxt.text = I2.Loc.LocalizationManager.CurrentLanguage;
    }

    private void NextButton()
    {
        var lastId = currentLanguageId;
        currentLanguageId++;
        UpdateVisual();

        UpdateLanguage(currentLanguageId);
    }
}