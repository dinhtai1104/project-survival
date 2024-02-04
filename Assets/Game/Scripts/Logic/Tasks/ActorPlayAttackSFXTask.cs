using Cysharp.Threading.Tasks;
using Game.GameActor;
using Game.Tasks;

public class ActorPlayAttackSFXTask : Task
{
    public override async UniTask Begin()
    {
        await base.Begin();
        PlaySound();
        IsCompleted = true;
    }

    public void PlaySound()
    {
        try
        {
            var Caster = GetComponentInParent<ActorBase>();
            Caster.SoundHandler.PlayOneShot(Caster.soundData.engageSFXs.Random(), 1);
        }
        catch (System.Exception e)
        {

        }
    }
   
}


