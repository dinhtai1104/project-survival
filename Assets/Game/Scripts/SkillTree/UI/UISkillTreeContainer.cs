using Assets.Game.Scripts._Services;
using Assets.Game.Scripts.BaseFramework.Architecture;
using com.mec;
using Cysharp.Threading.Tasks;
using Mosframe;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.UI;

public class UISkillTreeContainer : MonoBehaviour
{
    [SerializeField] private DynamicHScrollView scrollView;
    [SerializeField] private RectTransform bgUnlock;
    [SerializeField] private RectTransform bgLock;
    [SerializeField] private Image imageFillProgress;
    private Dictionary<int, SkillTreeStageEntity> datas = new Dictionary<int, SkillTreeStageEntity>();
    private SkillTreeTable table;
    public int CurrentSkill { get; set; }
    [Button]
    public void Init()
    {
        table = DataManager.Base.SkillTree;
        PrepareData();

        scrollView.init(datas.Count);
        UpdateUI();
    }

    public async void UpdateUI()
    {
        int index = 0;
        foreach (var e in datas)
        {
            if (Architecture.Get<SkillTreeService>().IsUnlockSkill(e.Value.Level, e.Value.Index))
            {
                index++;
            }
            else
            {
                break;
            }
        }
        Timing.RunCoroutine(UpdateLayoutElement(index), gameObject);
       
        CurrentSkill = index - 1;
        await UniTask.Delay(TimeSpan.FromSeconds(0.1f));

        scrollView.scrollByItemIndex(CurrentSkill);
        scrollView.refresh();
    }

    private IEnumerator<float> UpdateLayoutElement(int index)
    {
        yield return Timing.WaitForSeconds(0.05f);
        var percentUnlock = index * 1.0f / datas.Count;
        var percentLock = 1 - percentUnlock;

        imageFillProgress.fillAmount = percentUnlock;

        bgUnlock.sizeDelta = new Vector2(index * 300, bgUnlock.GetHeight());
    }



    private void PrepareData()
    {
        int index = 0;
        foreach (var item in table.Dictionary)
        {
            int level = item.Key;
            var skills = item.Value;

            foreach (var skill in skills.Skills)
            {
                datas.Add(index, skill);
                index++;
            }
        }
    }

    public SkillTreeStageEntity Convert(int Index)
    {
        return datas.ContainsKey(Index) ? datas[Index] : default;
    }

}


// Data row skill tree \\
/*
 * 
 * Level - Id => Index
 * 
 * Dictionary<Level, List<Id>> Data
 * 
 * Index=>Data
 * 
 * 
 * 
 * 
 * 
 */
