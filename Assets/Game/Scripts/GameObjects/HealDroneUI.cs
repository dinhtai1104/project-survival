using Game.GameActor;
using Game.Skill;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealDroneUI : MonoBehaviour
{
    ActorBase Base;
    [SerializeField]
    private TMPro.TextMeshProUGUI amountText;
    Transform target, _transform;
    [SerializeField]
    private Transform panel;
    [SerializeField]
    private GameObject [] bars;
    Vector3 offset;

    public void SetUp(ActorBase Base,Transform target, Vector3 offset)
    {
        this.Base = Base;
        _transform = transform;
        this.offset = offset;
        this.target = target;
        var skill=Base.SkillEngine.GetSkill(0) as MultiTaskSkill;
        Update(skill.maxCastTime.IntValue-skill.totalCast);

        SetActive(true);

    }
    private void OnEnable()
    {
        Messenger.AddListener<ActorBase,int, int>(EventKey.OnCastSkill, OnCastSkill);

    }
    private void OnDisable()
    {
        Messenger.RemoveListener<ActorBase, int, int>(EventKey.OnCastSkill, OnCastSkill);

    }

    private void OnCastSkill(ActorBase caster, int current, int max)
    {
        if (caster != Base) return;
        if (current >= max)
        {
            panel.Shake(0.15f, 5, 0.15f);
        }
        Update((max - current)-1);
    }

    public void Show()
    {
        SetActive(true);
    }
    public void Hide()
    {
        SetActive(false);
    }
    public void SetActive(bool active)
    {
        gameObject.SetActive(active);
    }
    public void Update(int remainingPoint)
    {
        amountText.text = $"x{remainingPoint}";
        for(int i = 0; i < bars.Length; i++)
        {
            bars[i].SetActive(i<remainingPoint);
        }
    }
    private void FixedUpdate()
    {
        if (target == null) return;
        _transform.localPosition = target.position + offset;
    }
    private void Update()
    {
        
    }

}