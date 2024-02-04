using Cysharp.Threading.Tasks;
using Game.GameActor;

namespace Game.GameActor.Buff
{
    public class NullBuffHandler : IBuffHandler
    {
        public async UniTask Cast(EBuff buff, bool save = true)
        {
        }

        public async UniTask Cast(IBuff buff)
        {
            await UniTask.Yield();
        }

        public async UniTask CastOnSave(EBuff buff, bool use)
        {
            await UniTask.Yield();
        }

        public IBuff GetBuff(EBuff buff)
        {
            return null;
        }

        public bool HasBuff(EBuff buff)
        {
            return false;
        }

        public void Initialize(ActorBase caster)
        {
            return;
        }

        public void Remove(EBuff buff)
        {
        }

        public void Ticks()
        {
        }
    }
}