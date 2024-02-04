using UnityEngine;
using UnityEngine.UI;

public class UISetColorImage : MonoBehaviour
{
    private Image image;
    private void Awake()
    {
        image = GetComponent<Image>();
    }
    public void SetColor(Color color)
    {
        image.color = color;
    }
}