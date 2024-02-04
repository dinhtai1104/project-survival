using Game.GameActor;
using System;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : HealthBarBase
{
    [SerializeField]
    private Image hpBar;
    protected override void Init()
    {
    }

    protected override void OnArmorBroke()
    {
    }

    protected override void OnHealthDepleted()
    {
        //gameObject.SetActive(false);
    }

    protected override void OnUpdate(HealthHandler health)
    {
        hpBar.fillAmount = health.GetHealth()*1f / health.GetMaxHP();
    }
}