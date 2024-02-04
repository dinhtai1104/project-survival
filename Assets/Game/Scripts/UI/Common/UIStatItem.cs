using Assets.Game.Scripts.Utilities;
using com.mec;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIStatItem : MonoBehaviour
{
    public StatKey statKey;
    private IStatGroup statGroup;
    [SerializeField] private Image iconStatImg;
    [SerializeField] private TextMeshProUGUI statTxt;
    [SerializeField] protected TextMeshProUGUI statAddTxt;

    private int lastStat = -1;
    private bool isShowFirst = false;
    protected virtual void OnEnable()
    {
        statGroup = GameSceneManager.Instance.PlayerData.Stats;
        lastStat = (int)statGroup.GetValue(statKey);

        SetData(statKey, statGroup.GetValue(statKey));
        statGroup.AddListener(statKey, OnChangeStat);
    }

    public void SetData(StatKey statKey, float value)
    {
        iconStatImg.sprite = ResourcesLoader.Instance.GetSprite(AtlasName.Stat, statKey.ToString());
        statTxt.text = value.ToInt().TruncateValue();

        Timing.KillCoroutines(gameObject);
        if (isShowFirst)
            Timing.RunCoroutine(_AnimateScore((int)value), gameObject);
        lastStat = (int)value;
        isShowFirst = true;
    }
    public void SetData(StatAffix stat)
    {
        SetData(stat.StatName, stat.Value);
    }

    protected virtual void OnDisable()
    {
        statGroup.RemoveListener(statKey, OnChangeStat);
    }

    private void OnChangeStat(float value)
    {
        //statTxt.text = statGroup.GetValue(statKey, 0).ToInt().TruncateValue();

        Timing.KillCoroutines(gameObject);
        Timing.RunCoroutine(_AnimateScore((int)value), gameObject);
        lastStat = (int)value;
    }

    private IEnumerator<float> _AnimateScore(int to, float duration = 0.5f)
    {
        float time = 0;
        int lastStat = this.lastStat;
        if (lastStat != to)
        {
            while (time < duration)
            {
                var value = Mathf.Lerp(lastStat, to, time / duration);
                statTxt.text = ((int)value).TruncateValue();
                time += Time.deltaTime;
                yield return Timing.DeltaTime;
            }
        }
        statTxt.text = to.TruncateValue();
    }
    public virtual void SetAddStat(float data)
    {

    }
}