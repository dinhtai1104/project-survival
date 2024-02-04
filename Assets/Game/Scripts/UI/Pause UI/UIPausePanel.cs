using System;
using TMPro;
using UnityEngine;

public class UIPausePanel : UI.Panel
{
    [SerializeField] private TextMeshProUGUI currentWaveTxt;
    [SerializeField] private UIBuffCollectionView collectionView;
    public override void PostInit()
    {
        SetTextCurrentZone();
        SetCollectionView();
    }

    private void SetCollectionView()
    {
        var data = new BuffCollectionData();
        //data.BuffEquiped = DataManager.Save.Buff.Dungeon.BuffEquiped;
        data.BuffEquiped = GameController.Instance.GetSession().buffSession.Dungeon.BuffEquiped;

        collectionView.Show(data);
    }

    private void SetTextCurrentZone()
    {
        var str = I2Localize.GetLocalize("Pause/CurrentWave");
        var data = GameController.Instance.GetSession();
        var currentZone = data.CurrentDungeon + 1;
        var currentWave = data.CurrentStage + 1;
        var nameZone = I2Localize.GetLocalize($"Dungeon/Dungeon_{currentZone}");
        if (data.Mode == GameMode.DungeonEvent)
        {
            nameZone = I2Localize.GetLocalize($"Common/DungeonEvent_Title_{(data as DungeonEventSessionSave).Type}");
        }
        str = string.Format(str, currentZone, nameZone, currentWave, data.MemoryMap.roomId.Count);

        currentWaveTxt.text = str;
    }

    
}
