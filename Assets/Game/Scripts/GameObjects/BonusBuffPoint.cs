using Cysharp.Threading.Tasks;
using MoreMountains.Feedbacks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusBuffPoint : MonoBehaviour
{
    public GameObject chest;
    public MMF_Player startFb;
    public void Trigger() 
    {
        GameUIPanel.Instance.inputController.SetActive(false);
        Invoke(nameof(ShowBuff),1);
    }
    public void SpawnChest()
    {
        chest.transform.localPosition = new Vector3(0, 4f);
        chest.SetActive(true);
    }
    void ShowBuff()
    {
        UI.PanelManager.Create(AddressableName.UIBuffChoiceAngelWhisper, panel =>
        {
            panel.Show();
        });
        //test
        //Invoke(nameof(OnBuffSelected), 1);
        Messenger.AddListener<EPickBuffType>(EventKey.PickBuffDone, OnBuffSelected);
    }

    void OnBuffSelected(EPickBuffType type)
    {
        GameUIPanel.Instance.inputController.SetActive(true);
        Messenger.Broadcast(EventKey.GameClear,false);
        //
        Messenger.RemoveListener<EPickBuffType>(EventKey.PickBuffDone, OnBuffSelected);
    }
    private void OnDisable()
    {
        chest.SetActive(false);
    }
    private async void OnEnable()
    {
        await UniTask.Delay(500);
        startFb?.PlayFeedbacks();
    }
}
