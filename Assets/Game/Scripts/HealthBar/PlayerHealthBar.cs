using Game.GameActor;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : HealthBarBase
{
    [SerializeField]
    private HealthPoint[] healthPoints;
    [SerializeField]
    private RectTransform backPanel;

    int totalBar;
    int[] markers = { 500, 1000, 5000, 10000 };
    int mimimum = 5;
    protected override void Init()
    {
        for (int i = 0; i < healthPoints.Length; i++)
        {
            healthPoints[i].Deactive();
        }
        float maxHp = actor.HealthHandler.GetMaxHP();
     
        int step = 0;
        for(int i = markers.Length - 1; i >= 0; i--)
        {
            if (maxHp >= markers[i])
            {
                step = i;
                break;
            }
        }
        totalBar = Mathf.Clamp(mimimum+step, 5, 8);

        backPanel.sizeDelta = new Vector2(Mathf.Clamp(totalBar*1f / mimimum, 1.4f, 1.9f), backPanel.sizeDelta.y);
        backPanel.GetComponent<GridLayoutGroup>().cellSize = new Vector2(backPanel.sizeDelta.x/totalBar, backPanel.sizeDelta.y);
        for (int i = 0; i < totalBar; i++)
        {
            healthPoints[i].Show();
            healthPoints[i].Set(1);
        }
        GetComponentInChildren<GridLayoutGroup>().enabled = false;
        Invoke(nameof(RefreshGrid), 0.05f);

    }
    void RefreshGrid()
    {
        GetComponentInChildren<GridLayoutGroup>().enabled = true;

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
        for(int i = totalBar - 1; i >= 0; i--)
        {
            float hp = (i)*health.GetMaxHP() / totalBar;
            healthPoints[i].Set(health.GetHealth()<=hp?0:(health.GetHealth()-hp)*1f/(health.GetMaxHP()/totalBar));
        }
        healthText.SetText(((int)health.GetHealth()).ToString());
    }

}
