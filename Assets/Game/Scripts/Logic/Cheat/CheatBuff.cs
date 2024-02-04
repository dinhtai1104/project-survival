using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Cysharp.Threading.Tasks;
using Game.GameActor;

public class CheatBuff : MonoBehaviour
{
    public TMP_Dropdown buffDropdown;
    private BuffTable buffTable;
    private Dictionary<int, EBuff> buffData = new Dictionary<int, EBuff>();
    private async void Start()
    {
        buffDropdown.ClearOptions();
        await UniTask.Delay(1000);
        buffTable = DataManager.Base.Buff;
        List<string> data = new List<string>();
        int count = 0;
        foreach (var item in buffTable.Dictionary)
        {
            data.Add(item.Value.Id + ". " + item.Value.Type.ToString());
            buffData.Add(count++, item.Value.Type);
        }
        buffDropdown.AddOptions(data);
    }

    public void CastAbility()
    {
        var buffType = buffData[buffDropdown.value];
        //abilityLive.Value.AcceptAbility(index);

        Debug.Log("Cast Debug: " + buffType);
        Messenger.Broadcast(EventKey.CastBuff, FindObjectOfType<BattleGameController>().player as ActorBase, buffType);
    }

    
}