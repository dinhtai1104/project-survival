using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class UIContentTab : MonoBehaviour
{
    private ScrollRect scrollRect;
    protected CancellationTokenSource cancelToken;

    private void Awake()
    {
        scrollRect = GetComponentInChildren<ScrollRect>();
    }
    public virtual void Show() 
    {
        cancelToken = new CancellationTokenSource();
        if (scrollRect != null)
        {
            scrollRect.normalizedPosition = Vector3.up;
        }
    }
}