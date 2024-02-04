using System;
using UnityEngine;
using UnityEngine.UI;

public class UIShinyInventorySlot : MonoBehaviour
{
    public float alpha=0.5f;
    public Image shinyImg;
    public void SetColor(Color color)
    {
        if (shinyImg != null)
        {
            color.a = alpha;
            shinyImg.color = color;
        }
    }
}