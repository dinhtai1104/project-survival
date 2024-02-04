using Cysharp.Threading.Tasks;
using Game.Tasks;
using UnityEngine;

public class SetActiveGameObjectTask : SkillTask
{
    public bool Active;
    public GameObject[] GO;
    public override async UniTask Begin()
    {
        await base.Begin();
        foreach (var go in GO)
        {
            go.SetActive(Active);
        }
        IsCompleted = true;
    }
}