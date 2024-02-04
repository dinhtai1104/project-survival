using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIShopOfferDungeon : UIShopOffer
{
    [SerializeField] private Button nextButton;
    [SerializeField] private Button previousButton;
    [SerializeField] private UIShopOfferDungeonItem offerItem;
    private OfferDungeonTable offer;
    private OfferSave offerSave;
    private string prefabPath => "Common/UIShopDungeonOfferItem.prefab";
    private int CurrentIdOffer;

    private void OnEnable()
    {
        nextButton.onClick.AddListener(NextOnClicked);
        previousButton.onClick.AddListener(PreviousOnClicked);
    }
    private void OnDisable()
    {
        nextButton.onClick.RemoveListener(NextOnClicked);
        previousButton.onClick.RemoveListener(PreviousOnClicked);
    }

    public override void OnInit()
    {
        base.OnInit();
        offer = DataManager.Base.OfferDungeon;
        offerSave = DataManager.Save.Offer.OfferDungeon;

        offerItem.Init(this);

        CurrentIdOffer = GetCurrentOffer();
        if (CurrentIdOffer == -1)
        {
            gameObject.SetActive(false);
            return;
        }
        else
        {
            gameObject.SetActive(true);
        }
        var currentDungeon = GetEntity(CurrentIdOffer);
        var saveOffer = offerSave.GetItem(CurrentIdOffer);
        offerItem.SetData(currentDungeon, saveOffer);

        previousButton.gameObject.SetActive(true);
        nextButton.gameObject.SetActive(true);

        var nextId = GetNextOffer();
        if (nextId == -1)
        {
            nextButton.gameObject.SetActive(false);
        }

        var previousId = GetPreviousOffer();
        if (previousId == -1)
        {
            previousButton.gameObject.SetActive(false);
        }
    }

    public void NextOnClicked()
    {
        var nextId = GetNextOffer();
        if (nextId == -1) return;

        CurrentIdOffer = nextId;
        var currentDungeon = GetEntity(CurrentIdOffer);
        var saveOffer = offerSave.GetItem(CurrentIdOffer);
        offerItem.SetData(currentDungeon, saveOffer);
        previousButton.gameObject.SetActive(true);
        nextButton.gameObject.SetActive(true);

        nextId = GetNextOffer();
        if (nextId == -1)
        {
            nextButton.gameObject.SetActive(false);
        }

        var previousId = GetPreviousOffer();
        if (previousId == -1)
        {
            previousButton.gameObject.SetActive(false);
        }
    }
    public void PreviousOnClicked()
    {
        var previousId = GetPreviousOffer();
        if (previousId == -1) return;

        CurrentIdOffer = previousId;
        var currentDungeon = GetEntity(CurrentIdOffer);
        var saveOffer = offerSave.GetItem(CurrentIdOffer);
        offerItem.SetData(currentDungeon, saveOffer);

        previousButton.gameObject.SetActive(true);
        nextButton.gameObject.SetActive(true);

        var nextId = GetNextOffer();
        if (nextId == -1)
        {
            nextButton.gameObject.SetActive(false);
        }

        previousId = GetPreviousOffer();
        if (previousId == -1)
        {
            previousButton.gameObject.SetActive(false);
        }
    }

    private OfferDungeonEntity GetEntity(int Id)
    {
        return offer.Get(Id);
    }

    private int GetNextOffer()
    {
        int result = -1;
        for (int i = 0; i < offerSave.Items.Count; i++)
        {
            var data = offerSave.Items[i];
            if (data.Id < CurrentIdOffer) continue;
            if (data.Id == CurrentIdOffer) continue;
            if (data.IsBoughtFirstTime)
            {
                continue;
            }
            result = data.Id;
            break;
        }
        return result;
    }
    private int GetPreviousOffer()
    {
        int result = -1;
        for (int i = offerSave.Items.Count - 1; i >= 0; i--)
        {
            var data = offerSave.Items[i];
            if (data.Id > CurrentIdOffer) continue;
            if (data.Id == CurrentIdOffer) continue;
            if (data.IsBoughtFirstTime)
            {
                continue;
            }
            result = data.Id;
            break;
        }
        return result;
    }
    private int GetCurrentOffer()
    {
        int result = -1;
        for (int i = 0; i < offerSave.Items.Count; i++)
        {
            var data = offerSave.Items[i];
            if (data.IsBoughtFirstTime)
            {
                continue;
            }
            result = data.Id;
            break;
        }
        return result;
    }
}