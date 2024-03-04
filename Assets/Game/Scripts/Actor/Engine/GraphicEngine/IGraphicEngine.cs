namespace Engine
{
    public interface IGraphicEngine
    {
        Actor Owner { get; }
        void Init(Actor actor);
        void SetActiveRenderer(bool active);
        void SetGraphicAlpha(float a);
        void SetFlashAmount(float amount);
        void FlashColor(float flickerDuration, int flickerAmount);
        void ClearFlashColor();
    }
}
