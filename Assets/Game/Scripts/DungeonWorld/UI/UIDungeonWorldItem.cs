using Assets.Game.Scripts.DungeonWorld.Data;
using Assets.Game.Scripts.Utilities;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIDungeonWorldItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameDungeonTxt;
    [SerializeField] private TextMeshProUGUI roomCountDungeonTxt;
    [SerializeField] private Image iconDungeon;
    [SerializeField] private GameObject enterButton, lockButton, claimedAllButton;
    [SerializeField] private RectTransform detailPanel;
    private DungeonEntity entity;
    private DungeonWorldEntity worldEntity;
    public int Dungeon => entity.Dungeon;
    public event Action<UIDungeonWorldItem> onClicked;

    private void OnEnable()
    {
        enterButton.GetComponent<Button>().onClick.AddListener(OnClicked);
        claimedAllButton.GetComponent<Button>().onClick.AddListener(OnClicked);
    }

    private void OnClicked()
    {
        onClicked?.Invoke(this);
    }

    private void OnDisable()
    {
        enterButton.GetComponent<Button>().onClick.RemoveListener(OnClicked);
        claimedAllButton.GetComponent<Button>().onClick.RemoveListener(OnClicked);
    }

    public void Set(DungeonEntity entity)
    {
        this.entity = entity;
        worldEntity = DataManager.Base.DungeonWorld.Get(entity.Dungeon);
        Setup();
    }

    public void Setup()
    {
        nameDungeonTxt.text = I2Localize.GetLocalize("Dungeon/Dungeon_" + (entity.Dungeon + 1));
        roomCountDungeonTxt.text = I2Localize.GetLocalize("Common/Title_World_RoomCount", entity.Stages.Count);
        enterButton.SetActive(false);
        lockButton.SetActive(false);
        claimedAllButton.SetActive(false);

        iconDungeon.sprite = ResourcesLoader.Instance.GetSprite(AtlasName.DungeonIcon, "Dungeon_" + (entity.Dungeon));

        if (DataManager.Save.Dungeon.CanPlayDungeon(entity.Dungeon))
        {
            if (DataManager.Save.DungeonWorld.IsClaimAll(entity.Dungeon))
            {
                claimedAllButton.SetActive(true);
            }
            else
            {
                enterButton.SetActive(true);
            }
        }
        else
        {
            lockButton.SetActive(true);
        }
    }

    public void Register(UIDungeonWorldReward rewardPanel)
    {
        rewardPanel.transform.SetParent(detailPanel);
        rewardPanel.Set(worldEntity);
        enterButton.SetActive(false);
        rewardPanel.gameObject.SetActive(true);
    }
}