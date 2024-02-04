using Assets.Game.Scripts._Services;
using Assets.Game.Scripts.BaseFramework.Architecture;
using Assets.Game.Scripts.Subscription.Services;
using UnityEngine;

public class UIAdIcon : MonoBehaviour
{
    private void OnEnable()
    {
        if (Architecture.Get<AdService>().IsFreeRewardAd)
        {
            gameObject.SetActive(false);
        }
    }
}