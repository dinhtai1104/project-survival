namespace Engine
{
    public interface IGraphicEngine
    {
        ActorBase Owner { get; }
        void Init(ActorBase actor);
        void SetActiveRenderer(bool active);
        void SetGraphicAlpha(float a);
        void SetFlashAmount(float amount);
        void FlashColor(float flickerDuration, int flickerAmount);
        void ClearFlashColor();
    }
}
