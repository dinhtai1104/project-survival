using Cysharp.Threading.Tasks;
using Game.Level;
using System;
using TMPro;
using UnityEngine;
public class UICooldownWave : MonoBehaviour
{
    [SerializeField] private GameObject cooldownGO;
    [SerializeField] private TextMeshProUGUI currentWaveTxt;
    [SerializeField] private TextMeshProUGUI cooldownTimeTxt;
    private int currentWave;

    private void OnEnable()
    {
        Messenger.AddListener<Callback>(EventKey.StageStart, OnStageStart);
        Messenger.AddListener<DungeonWaveEntity, float>(EventKey.ShowCooldownWave, OnShowCooldownWave);
        cooldownGO.SetActive(true);
        currentWaveTxt.text = cooldownTimeTxt.text = "";
    }
    private void OnDisable()
    {
        Messenger.RemoveListener<Callback>(EventKey.StageStart, OnStageStart);
        Messenger.RemoveListener<DungeonWaveEntity, float>(EventKey.ShowCooldownWave, OnShowCooldownWave);
    }

    private void OnShowCooldownWave(DungeonWaveEntity wave, float delay)
    {
        if (delay == -1)
        {
            currentWaveTxt.text = cooldownTimeTxt.text = "";
            return;
        }
        currentWaveTxt.text = I2Localize.GetLocalize("Common/Cooldown_Next_Wave") + (wave.WaveId + 1).ToString() + "/" + wave.RoomLink.Waves.Count;
        cooldownTimeTxt.text = I2Localize.GetLocalize("Common/Cooldown_LeftTime_Wave") + new TimeSpan(0, 0, delay.ToInt()).ConvertTimeToString();
    }

    private void OnStageStart(Callback cb)
    {
        cooldownGO.SetActive(true);
        currentWaveTxt.text = cooldownTimeTxt.text = "";
    }
}
