using Assets.Game.Scripts.Utilities;
using Cysharp.Threading.Tasks;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UISkillTreeResultPanel : UI.Panel
{
    [SerializeField] private Image m_SkillIconImg;
    [SerializeField] private TextMeshProUGUI m_SkillDescriptionText;
    [SerializeField] private TextMeshProUGUI m_SkillNameTxt;
    private bool canClose = false;
    public override void PostInit()
    {
    }

    public async void Show(SkillTreeStageEntity entity)
    {
        base.Show();
        m_SkillIconImg.sprite = ResourcesLoader.Instance.GetSprite(AtlasName.SkillTree, $"{entity.Modifier.StatKey}");
        m_SkillDescriptionText.text = entity.Modifier.ToString();
        m_SkillNameTxt.text = I2Localize.GetLocalize($"Stat/{entity.Modifier.StatKey}");

        await UniTask.Delay(TimeSpan.FromSeconds(0.5f));
        canClose = true;
    }
    public override void Close()
    {
        if (canClose)
        {
            base.Close();
        }
    }
}
