using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveInfoView : MonoBehaviour
{
    [SerializeField]
    private TMPro.TextMeshProUGUI timeText,coinText,gemText,levelText,currentXPText;
    [SerializeField]
    private Image xpBar;

    public CloudSave.ESaveType saveType;
    public CloudSave.SavePackage save;
    System.Action<CloudSave.ESaveType, CloudSave.SavePackage> onSelected;
    public void SetUp(CloudSave.ESaveType saveType, CloudSave.SavePackage save,System.Action<CloudSave.ESaveType,CloudSave.SavePackage> onSelected)
    {
        this.onSelected = onSelected;
        this.save = save;
        this.saveType = saveType;

        timeText.text = $"Date: {save.timeStamp}";
        coinText.SetText(save.coin.ToString());
        gemText.SetText(save.gem.ToString());
        levelText.SetText(save.level.ToString());

        var expHandler= GameSceneManager.Instance.PlayerData.ExpHandler;
        var currentLvlExp = expHandler.LevelToExp(save.level);
        var nextLvlExp = expHandler.LevelToExp(Mathf.Clamp(save.level + 1, 1, expHandler.MaxLevel));
        float exp = save.xp - currentLvlExp;
        float total = nextLvlExp - currentLvlExp;
        float percentage = exp / total;

        currentXPText.SetText(exp.ToString());
        xpBar.fillAmount = percentage;


    }
    public void Select()
    {
        onSelected?.Invoke(saveType,save);
    }
}


