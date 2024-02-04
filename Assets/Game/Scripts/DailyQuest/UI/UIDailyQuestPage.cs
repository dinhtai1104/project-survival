using Assets.Game.Scripts._Services;
using Assets.Game.Scripts.BaseFramework.Architecture;
using com.mec;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Foundation.Game.Time;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class UIDailyQuestPage : UIContentTab
{
    [SerializeField] private UIDailyQuestMilestoneBar milestoneBar;
    [SerializeField] private UIDailyQuestItem itemPrefab;
    [SerializeField] private RectTransform contentMission;
    [SerializeField] private TextMeshProUGUI titleTxt;
    [SerializeField] private TextMeshProUGUI timeLeftTxt;

    [SerializeField] private RectTransform medalTargetRect;
    [SerializeField] private RectTransform medalPrefab;

    private List<UIDailyQuestItem> allItems;
    private DailyQuestTable table;
    private QuestService service;
    private DailyQuestProgressTable progress;
    private bool isInit = false;

    [SerializeField] private AnimationCurve appearCurve;
    [SerializeField] private AnimationCurve moveCurve;

    private List<DailyQuestSave> saveList = new List<DailyQuestSave>();

    [Button]
    public override void Show()
    {
        base.Show();
        table = DataManager.Base.DailyQuest;
        progress = DataManager.Base.DailyQuestProgress;
        service = Architecture.Get<QuestService>();

        milestoneBar.Setup();
        if (!isInit)
        {
            isInit = true;
            allItems = new List<UIDailyQuestItem>();

            foreach (var model in table.Dictionary)
            {
                saveList.Add(service.GetSave(model.Key));
            }

            Prepare();
        }
        UpdateUI();
        Timing.KillCoroutines(gameObject);
        Timing.RunCoroutine(_Ticks(), gameObject);
    }

    private IEnumerator<float> _Ticks()
    {
        var general = DataManager.Save.General;
        var endDay = new System.TimeSpan(24, 0, 0);
        while (true)
        {
            var now = UnbiasedTime.UtcNow.TimeOfDay;
            var left = endDay - now;
            timeLeftTxt.text = I2Localize.GetLocalize("Common/TimeLeft_Quest_Title") + " " + left.ConvertTimeToString();
            if (left.TotalSeconds <= 1) break;
            yield return Timing.WaitForSeconds(1f);
        }
        DataManager.Save.NextDay();
        Show();
    }

    private void OnDisable()
    {
        cancelToken.Cancel();
        cancelToken.Dispose();
        Timing.KillCoroutines(gameObject);
        PoolManager.Instance.Clear(medalPrefab.gameObject);
    }
    private void OnDestroy()
    {
        if (cancelToken != null)
        {
            cancelToken.Cancel();
            cancelToken.Dispose();
            cancelToken = null;
        }   
    }
    public void UpdateUI()
    {
        saveList = saveList.OrderByDescending(save =>
        {
            return this.service.CanReceive(save.Id);
        }).ThenBy(save =>
        {
            return this.service.IsReceiveMission(save.Id);
        }).ToList();
        int index = 0;
        foreach (var item in allItems)
        {
            item.Setup(GetComponentInParent<UIQuestPanel>(), DataManager.Base.DailyQuest.Get(saveList[index++].Id));
            item.UpdateUI();
        }
        milestoneBar.UpdateUI();
    }

    private void Prepare()
    {
        saveList = saveList.OrderByDescending(save =>
        {
            return this.service.CanReceive(save.Id);
        }).ThenBy(save =>
        {
            return this.service.IsReceiveMission(save.Id);
        }).ToList();
        foreach (var model in saveList)
        {
            var item = PoolManager.Instance.Spawn(itemPrefab, contentMission);
            item.Setup(GetComponentInParent<UIQuestPanel>(), DataManager.Base.DailyQuest.Get(model.Id));
            allItems.Add(item);
        }
    }

    public async void DOAnimationMedalToTarget(Vector2 from)
    {
        List<RectTransform> list = new List<RectTransform>();
        var spawnCoinTaskList = new List<UniTask>();
        for (int i = 0; i < 10; i++)
        {
            var obj = PoolManager.Instance.Spawn(medalPrefab, PanelManager.Instance.transform);

            float rdOffX = UnityEngine.Random.Range(-3, 3f);
            float rdOffY = UnityEngine.Random.Range(-3, 3f);
            var posStart = Vector2.zero + new Vector2(rdOffX, rdOffY);
            obj.transform.position = posStart;
            list.Add(obj);
            spawnCoinTaskList.Add(obj.transform.DOPunchPosition(new Vector3(0, 15, 0), UnityEngine.Random.Range(0, 0.5f)).SetEase(Ease.InOutElastic)
              .AsyncWaitForCompletion().AsUniTask());
            await UniTask.Delay(TimeSpan.FromSeconds(0.01f), cancellationToken: cancelToken.Token);
        }
        await UniTask.WhenAll(spawnCoinTaskList).AttachExternalCancellation(cancelToken.Token);

        List<UniTask> moveCoinTask = new List<UniTask>();
        var pos = medalTargetRect.transform.position;
        for (int i = list.Count - 1; i >= 0; i--)
        {
            moveCoinTask.Add(MoveCoinTask(list[i], pos));
            await UniTask.Delay(TimeSpan.FromSeconds(0.05f), cancellationToken: cancelToken.Token);
        }
        await UniTask.WhenAll(moveCoinTask).AttachExternalCancellation(cancelToken.Token);
    }

    private async UniTask MoveCoinTask(RectTransform coinInstance, Vector3 pos)
    {
        await coinInstance.transform.DOMove(pos, 0.7f).SetEase(Ease.InBack).AsyncWaitForCompletion().AsUniTask().AttachExternalCancellation(cancelToken.Token);
        PoolManager.Instance.Despawn(coinInstance.gameObject);
    }
}