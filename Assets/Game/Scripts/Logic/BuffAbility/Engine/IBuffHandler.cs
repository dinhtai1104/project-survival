using Cysharp.Threading.Tasks;
using Game.GameActor;

namespace Game.GameActor.Buff
{
    public interface IBuffHandler
    {
        void Initialize(ActorBase caster);
        IBuff GetBuff(EBuff buff);
        UniTask Cast(EBuff buff, bool save = true);
        UniTask CastOnSave(EBuff buff, bool use);
        UniTask Cast(IBuff buff);
        bool HasBuff(EBuff buff);
        void Ticks();
        void Remove(EBuff buff);
    }
}