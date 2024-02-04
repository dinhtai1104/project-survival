using Cysharp.Threading.Tasks;
using System;

public class ActorHideTask : SkillTask
{
    public bool hide = true;

    public float time = 0;
    public override async UniTask Begin()
    {
        await base.Begin();
        IsCompleted = true;
        await UniTask.Delay(TimeSpan.FromSeconds(time));
        if (hide)
        {
            //Caster.gameObject.SetActive(!hide);
            Caster.Tagger.AddTag(ETag.Immune);
        }
        else
        {
            Caster.Tagger.RemoveTag(ETag.Immune);
        }
    }
}