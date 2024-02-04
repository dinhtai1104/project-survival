using com.mec;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UITryHeroCollectionItem : UITryHeroElement
{
    protected TryHeroSave tryHeroSave;
    [SerializeField] private TextMeshProUGUI cooldownHeroTxt;
    [SerializeField] private TextMeshProUGUI passiveTxt;
    [SerializeField] private GameObject cooldownGameObject;
    [SerializeField] private GameObject activeGameObject;

    private CoroutineHandle tickCooldownHandler;

    public delegate void ActiveHeroDelegate();
    public ActiveHeroDelegate onActivatedHero;
    public override void Init(EHero heroType, bool owner = false)
    {
        base.Init(heroType);
        this.tryHeroSave = DataManager.Save.TryHero.Saves[heroType];
        passiveTxt.text = I2Localize.GetLocalize($"Hero_Description/{tryHeroSave.Hero}");
        Timing.KillCoroutines(tickCooldownHandler);

        cooldownGameObject.SetActive(true);
        activeGameObject.SetActive(false);

        tickCooldownHandler = Timing.RunCoroutine(_TicksCooldown());
    }

    private IEnumerator<float> _TicksCooldown()
    {
        var nextTimeActivated = tryHeroSave.NextTimeUnlock;
        while (true)
        {
            var timeLeft = (nextTimeActivated - DateTime.Now);
            var totalSecond = timeLeft.TotalSeconds;
            cooldownHeroTxt.text = I2Localize.GetLocalize("Common/Title_FreeIn") + timeLeft.ConvertTimeToString();

            if (totalSecond <= 0)
            {
                break;
            }
            yield return Timing.WaitForSeconds(1f);
        }
       
        onActivatedHero?.Invoke();
        OnActiveHero();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        Timing.KillCoroutines(tickCooldownHandler);
    }

    private void OnActiveHero()
    {
        cooldownGameObject.SetActive(false);
        activeGameObject.SetActive(true);
    }
}