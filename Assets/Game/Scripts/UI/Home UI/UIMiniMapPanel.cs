using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System;
using MagneticScrollView;
using com.mec;
using Game.GameActor;
using DG.Tweening;
using System.Security.Policy;

public class UIMiniMapPanel : MonoBehaviour
{
    [SerializeField] private UIMapItem mapItem;
    [SerializeField] private MagneticScrollRect m_ScrollRect;
    [SerializeField] private RectTransform m_Content;
    [SerializeField] private GameObject nextButton;
    [SerializeField] private GameObject backButton;
    private DungeonMapMenu liveData;
    private List<UIMapItem> m_Items;

    private DungeonTable m_DungeonTable;
    private DungeonSave m_DungeonSave;
    private int currentDungeonId;
    private int maxDungeon;

    private void OnEnable()
    {
        nextButton.GetComponent<Button>().onClick.AddListener(NextOnClicked);
        backButton.GetComponent<Button>().onClick.AddListener(PreviousOnClicked);
        Messenger.AddListener(EventKey.ClaimDungeonWorld, UpdateMap);
    }
    private void OnDisable()
    {
        nextButton.GetComponent<Button>().onClick.RemoveListener(NextOnClicked);
        backButton.GetComponent<Button>().onClick.RemoveListener(PreviousOnClicked);
        Messenger.RemoveListener(EventKey.ClaimDungeonWorld, UpdateMap);
    }

    public void Setup()
    {
        liveData = DataManager.Live.DungeonLive;
        m_DungeonTable = DataManager.Base.Dungeon;
        m_DungeonSave = DataManager.Save.Dungeon;
        m_Items = new List<UIMapItem>();
        maxDungeon = m_DungeonTable.Dictionary.Count - 1;
        for (int i = 0; i < maxDungeon; i++)
        {
            var item = GetDungeonMap(i);
            m_Items.Add(item);
        }

        // Comming soon
        GetDungeonCommingSoon(-1);
        maxDungeon += 1;

        // Scroll to current dungeon
        currentDungeonId = m_DungeonSave.CurrentDungeon;
        currentDungeonId = Mathf.Clamp(currentDungeonId, 0, maxDungeon - 2);

        ScrollToDungeon(currentDungeonId);
        liveData.CurrentDungeon = currentDungeonId;

        Messenger.Broadcast(EventKey.SelectMap, currentDungeonId);
        backButton.SetActive(currentDungeonId > 0);
        nextButton.SetActive(currentDungeonId < maxDungeon - 1);
    }

    private void GetDungeonCommingSoon(int map)
    {
        var path = "Map/UIMapItem_{0}.prefab".AddParams(map);
        var item = ResourcesLoader.Instance.Get<UIMapItem>(path, m_Content);
        item.Setup(dungeonId: map);
        m_Items.Add(item);
    }

    private UIMapItem GetDungeonMap(int map)
    {
        var path = "Map/UIMapItem_{0}.prefab".AddParams(map + 1);
        var item = ResourcesLoader.Instance.Get<UIMapItem>(path, m_Content);
        item.Setup(dungeonId: map);
        return item;
    }

    public void ScrollToDungeon(int currentZone)
    {
        scrolling = true;
        Timing.RunCoroutine(_ScrollToCurrent(currentZone), gameObject);
    }
    private bool scrolling = false;
    private IEnumerator<float> _ScrollToCurrent(int idZone)
    {

        int index = 0;
        int indexZone = 0;
        foreach (var item in m_Items)
        {
            if (item.DungeonId == idZone)
            {
                indexZone = index;
                continue;
            }
            else index++;
            item.transform.DOScale(Vector3.one * 0.8f, 0.2f);
            item.DOKill();
            item.DOFade(0, item.GetComponent<CanvasGroup>().alpha, 0.15f).OnComplete(()=>
            {
            });

        }

        m_ScrollRect.StartAutoArranging();
        m_ScrollRect.ResetScroll();
        yield return Timing.WaitForSeconds(0.1f);

        m_Items[indexZone].transform.DOScale(Vector3.one, 0.2f);
        m_Items[indexZone].DOKill();
        m_Items[indexZone].DOFade(1, 0, 0.15f);

        if (m_ScrollRect.gameObject.activeInHierarchy)
        {
            m_ScrollRect.ScrollTo(indexZone);
            m_ScrollRect.StopAutoArranging();
        }
        yield return Timing.WaitForSeconds(1f);
        scrolling = false;
    }

    public void NextOnClicked()
    {
        if (scrolling == true) return;

        currentDungeonId++;
        currentDungeonId = Mathf.Clamp(currentDungeonId, 0, maxDungeon - 1);

        foreach (var item in m_Items)
        {
            item.transform.DOScale(Vector3.one * 0.8f, 0.2f);
            item.DOKill();
            item.DOFade(0, item.GetComponent<CanvasGroup>().alpha, 0.15f).OnComplete(() =>
            {
            });
        }

        m_Items[currentDungeonId].transform.DOScale(Vector3.one, 0.2f).From(Vector3.zero);
        m_Items[currentDungeonId].DOKill();
        m_Items[currentDungeonId].DOFade(1, 0, 0.15f);


        m_ScrollRect.ScrollTo(currentDungeonId);
        liveData.CurrentDungeon = currentDungeonId;

        Messenger.Broadcast(EventKey.SelectMap, m_Items[currentDungeonId].DungeonId);

        backButton.SetActive(currentDungeonId > 0);
        nextButton.SetActive(currentDungeonId < maxDungeon - 1);
    }
    public void PreviousOnClicked()
    {
        if (scrolling == true) return;
        //m_Items[currentDungeonId].transform.DOScale(Vector3.one * 0.8f, 0.3f);
        //m_Items[currentDungeonId].DOKill();
        //m_Items[currentDungeonId].DOFade(0, 1, 0.3f);
        currentDungeonId--;
        currentDungeonId = Mathf.Clamp(currentDungeonId, 0, maxDungeon - 1);
        int index = 0;
        int indexZone = 0;
        foreach (var item in m_Items)
        {
            item.transform.DOScale(Vector3.one * 0.8f, 0.2f);
            item.DOKill();
            item.DOFade(0, item.GetComponent<CanvasGroup>().alpha, 0.2f).OnComplete(() =>
            {
            });
        }

        m_Items[currentDungeonId].transform.DOScale(Vector3.one, 0.2f).From(Vector3.zero);

        m_Items[currentDungeonId].DOKill();
        m_Items[currentDungeonId].DOFade(1, 0, 0.2f);

        m_ScrollRect.ScrollTo(currentDungeonId);
        liveData.CurrentDungeon = currentDungeonId;

        Messenger.Broadcast(EventKey.SelectMap, m_Items[currentDungeonId].DungeonId);

        backButton.SetActive(currentDungeonId > 0);
        nextButton.SetActive(currentDungeonId < maxDungeon - 1);
    }

    public UIMapItem GetCurrentMap()
    {
        return m_Items.Find(t => t.DungeonId == m_DungeonSave.CurrentDungeon);
    }

    public void UpdateMap()
    {
        m_Items.ForEach(t => t.UpdateMap());
    }
}