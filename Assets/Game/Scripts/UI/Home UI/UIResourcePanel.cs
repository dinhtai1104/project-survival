using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Cysharp.Threading.Tasks;
using UnityEngine.EventSystems;
using Assets.Game.Scripts.Utilities;

public class UIResourcePanel : MonoBehaviour, IPointerClickHandler
{
    public EResource Type;
    [SerializeField] private Image currencyImg;
    [SerializeField] private TextMeshProUGUI valueTxt;

    protected virtual void OnEnable()
    {
        // First init
        OnUpdateCurrency(Type);
        Messenger.AddListener<EResource>(EventKey.UpdateResource, OnUpdateCurrency);
        UpdateIconCurrency();
    }

    private void UpdateIconCurrency()
    {
        var spriteIcon = ResourcesLoader.Instance.GetSprite(AtlasName.Resources, Type.ToString());
        currencyImg.sprite = spriteIcon;
    }

    protected virtual void OnDisable()
    {
        Messenger.RemoveListener<EResource>(EventKey.UpdateResource, OnUpdateCurrency);
    }

    private void OnUpdateCurrency(EResource type)
    {
        if (type != Type) return;
        try
        {
            var resourceSave = DataManager.Save.Resources;
            var value = resourceSave.GetResource(type);
            valueTxt.text = value.TruncateValue();

            if (type == EResource.Energy)
            {
                valueTxt.text = $"{value.TruncateValue()}/{EnergyService.Instance.Capacity.Value}";
            }
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
#if DEVELOPMENT
        var save = DataManager.Save.Resources;
        save.IncreaseResource(Type, 1000);
#endif
    }
}