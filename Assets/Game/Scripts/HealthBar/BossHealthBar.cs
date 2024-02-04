using Cysharp.Threading.Tasks;
using Game.GameActor;
using MoreMountains.Feedbacks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossHealthBar : HealthBarBase
{
    [SerializeField]
    private Image hpBar;
    [SerializeField]
    private ParticleSystem hitPS;
    [SerializeField]
    RectTransform main,tip;
    Vector2 defaultPos,defaultScale;
    [SerializeField]
    Game.Pool.UIPoolHandler pool;

    [SerializeField]
    private MMF_Player triggerFB;

    float lastHP = 0;
    protected override void Init()
    {
        main = transform.GetChild(0).GetComponent<RectTransform>();
        defaultPos = main.anchoredPosition;

        defaultScale = hpBar.rectTransform.rect.size;
        lastHP = actor.HealthHandler.GetMaxHP();
    }   

    protected override void OnArmorBroke()
    {
    }
    public override void Hide()
    {
        GetComponent<Animator>().SetTrigger("Close");
    }
    public void Deactive()
    {
        SetActive(false);
    }
    protected override void OnHealthDepleted()
    {
        //gameObject.SetActive(false);
    }
    protected override void Update()
    {
    }
    Vector2 tipPosition = Vector2.zero;
    protected override void OnUpdate(HealthHandler health)
    {
        if (main == null) return;
        float percentage = health.GetHealth() * 1f / health.GetMaxHP();
        defaultScale = hpBar.rectTransform.rect.size;
        tipPosition.x = (-defaultScale.x / 2f + defaultScale.x * percentage);
        var hitEffect=pool.Get(0).GetComponent<HitDropEffect>();

        float change = Mathf.Abs(lastHP-health.GetHealth());

        hitEffect.SetUp(tipPosition, (defaultScale.x * (change * 1f / health.GetMaxHP())));

        healthText.text = ((int)health.GetHealth()).ToString();
        hpBar.fillAmount = percentage;
        //main.Shake(0.12f, 5, 0.4f).ContinueWith(()=> main.anchoredPosition = defaultPos).Forget();
        triggerFB?.PlayFeedbacks();
        tip.anchoredPosition = tipPosition;
        if (change > 0)
        {
            hitPS.Play();
        }
        lastHP = health.GetHealth();

    }
}