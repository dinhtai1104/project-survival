using com.mec;
using Foundation.Game.Time;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIResourceEnergyPanel : UIResourcePanel
{
    [SerializeField] private TextMeshProUGUI timeCooldownText;
    protected override void OnEnable()
    {
        base.OnEnable();
        Timing.RunCoroutine(_Update(), Segment.RealtimeUpdate, gameObject);
    }

    private IEnumerator<float> _Update()
    {
        var res = DataManager.Save.Resources;

        while (true)
        {
            if (res.GetResource(EResource.Energy) >= EnergyService.Instance.Capacity.Value)
            {
                timeCooldownText.enabled = false;
                yield return Timing.DeltaTime;
                continue;
            }
            else
            {
                timeCooldownText.enabled = true;
            }

            //UpdateTimeEnergy
            timeCooldownText.text = EnergyService.Instance.GetCooldownTime();
            yield return Timing.WaitForSeconds(1f);
        }
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        Timing.KillCoroutines(gameObject);
    }
}