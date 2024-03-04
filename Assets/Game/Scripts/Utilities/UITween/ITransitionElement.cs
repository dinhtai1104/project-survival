using Cysharp.Threading.Tasks;

public interface ITransitionElement
{
    void Init();
    UniTask Show();
    UniTask AutoShow();
    UniTask Hide();
    UniTask AutoHide();
    bool IsCompleted { get; }
}