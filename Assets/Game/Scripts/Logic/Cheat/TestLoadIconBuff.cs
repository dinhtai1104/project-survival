using Assets.Game.Scripts.Utilities;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class TestLoadIconBuff : MonoBehaviour
{
    public Button loadButton;
    public Image imageBuff;
    public TMP_Dropdown buffDropdown;
    private Dictionary<int, EBuff> buffData = new Dictionary<int, EBuff>();

    private async void Start()
    {
        loadButton.onClick.AddListener(OnLoad);
        await UniTask.Delay(1000);

        buffDropdown.ClearOptions();
        var buffTable = DataManager.Base.Buff;
        List<string> data = new List<string>();
        foreach (var item in buffTable.Dictionary)
        {
            data.Add(item.Value.Id + ". " + item.Value.Type.ToString());
            buffData.Add(item.Value.Id, item.Value.Type);
        }
        buffDropdown.AddOptions(data);
    }

    private void OnLoad()
    {
        var buffTable = DataManager.Base.Buff;
        var buffType = buffData[buffDropdown.value];
        var entity = buffTable.Dictionary[buffType];
        var sprite = ResourcesLoader.Instance.GetSprite(AtlasName.Buff, entity.Icon);
        imageBuff.sprite = sprite;
        imageBuff.SetNativeSize();
    }
}