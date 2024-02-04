using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIStageInfo : MonoBehaviour
{
    [SerializeField]
    private TMPro.TextMeshProUGUI waveText, timerText;
    private void OnEnable()
    {
        waveText.SetText(string.Empty);
        timerText.SetText(string.Empty);

        Messenger.AddListener<int,int>(EventKey.WaveStart, OnWaveStart);
        Messenger.AddListener<int>(EventKey.WaveCountDown, OnWaveCountDown);
        
    }

    private void OnWaveCountDown(int time)
    {
        if (time > 0)
        {
            timerText.text = $"{I2Localize.GetLocalize("Common/Cooldown_LeftTime_Wave")} {time}s";
        }
        else
        {
            timerText.SetText(string.Empty);
        }
    }

    private void OnWaveStart(int wave, int totalWave)
    {
        timerText.SetText(string.Empty);
        waveText.text = $"{I2Localize.GetLocalize("Common/Cooldown_Next_Wave")} {(wave+1).ToString()}/{totalWave}";
        CancelInvoke();
        Invoke(nameof(DeactiveWaveNotice), 2);
    }
    void DeactiveWaveNotice()
    {
        waveText.SetText(string.Empty);
    }

    private void OnDisable()
    {
        CancelInvoke();
        Messenger.RemoveListener<int,int>(EventKey.WaveStart, OnWaveStart);
    }
    private void OnDestroy()
    {
        CancelInvoke();
        Messenger.RemoveListener<int,int>(EventKey.WaveStart, OnWaveStart);
    }
     

}
