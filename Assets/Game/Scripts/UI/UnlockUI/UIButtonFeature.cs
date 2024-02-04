using Cysharp.Threading.Tasks;
using System;
using UI;
using UnityEngine;

public class UIButtonFeature : MonoBehaviour
{
    public EFeature Feature;
    private IUnlockable unlockCondition;

    public void InitFirst()
    {
        transform.localScale = Vector3.one;
        unlockCondition = GetComponent<IUnlockable>();
        if (unlockCondition.Validate())
        {
            gameObject.SetActive(true);
            if (!DataManager.Save.ButtonFeature.IsUnlock(Feature))
            {
                GetComponent<CanvasGroup>().alpha = 0;
            }
            else
            {
                GetComponent<CanvasGroup>().alpha = 1;
            }
        }
        else
        {   
            gameObject.SetActive(false);
        }
    }

    public async UniTask Init()
    {
        unlockCondition = GetComponent<IUnlockable>();
        if (unlockCondition.Validate())
        {
            gameObject.SetActive(true);
            if (!DataManager.Save.ButtonFeature.IsUnlock(Feature))
            {
                MenuGameScene.Instance.EnQueue(Feature);
                GetComponent<CanvasGroup>().alpha = 0;

                //    var ui = await PanelManager.CreateAsync<UIUnlockFeaturePanel>(AddressableName.UIUnlockFeaturePanel);
                //    ui.Show(Feature);
                //    var close = false;
                //    ui.onClosed += () => close = true;
                //    await UniTask.WaitUntil(() => close);
                //    await UniTask.Delay(TimeSpan.FromSeconds(0.5f));
                //    DataManager.Save.ButtonFeature.Unlock(Feature);
                //}
            }
            else
            {
                GetComponent<CanvasGroup>().alpha = 1;
            }
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}