using UnityEngine;
using UnityEngine.UI;

public class UIBaseSlotItem : MonoBehaviour
{
    [SerializeField] private Image iconImg;
    public void SetIcon(Sprite sprite, bool setNativeSize = false)
    {
        iconImg.sprite = sprite;
        if (setNativeSize)
        {
            iconImg.SetNativeSize();
        }
    }
}