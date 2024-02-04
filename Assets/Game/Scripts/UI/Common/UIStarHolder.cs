using Assets.Game.Scripts.Utilities;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UIStarHolder : MonoBehaviour
{
    public enum SizeType
    {
        Fixed,
        None,
        NativeSize,
    }
    public SizeType Type = SizeType.None;
    [ShowIf(nameof(Type), SizeType.Fixed)] public Vector2 size;
    [SerializeField] private Image starPrefab;
    [SerializeField] private Transform holderTrans;

    private List<Image> stars = new List<Image>();
    private int star = 0;
    private int starsActive = 0;

    public void SetStar(int star)
    {
        starsActive = 0;
        this.star = star;
        ShowStar();
    }

    private void ShowStar()
    {
        foreach (var item in stars)
        {
            PoolManager.Instance.Despawn(item.gameObject);
        }
        stars.Clear();
        for (int i = 0; i < star; i++)
        {
            var starIns = PoolManager.Instance.Spawn(starPrefab, holderTrans);
            if (Type == SizeType.Fixed)
            {
                starIns.GetComponent<RectTransform>().sizeDelta = size;
            }
            if (Type == SizeType.NativeSize)
            {
                starIns.SetNativeSize();
            }
            stars.Add(starIns);
        }
    }
    public void SetImageStar(int index, Sprite spriteStar, bool active)
    {
        if (index >= stars.Count) return;
        stars[index].sprite = spriteStar;

        if (Type == SizeType.Fixed)
        {
            stars[index].GetComponent<RectTransform>().sizeDelta = size;
        }
        if (Type == SizeType.NativeSize)
        {
            stars[index].SetNativeSize();
        }

        if (active)
            starsActive++;
    }

    public void SetStarsActive(int count)
    {
        var starOn = ResourcesLoader.Instance.GetSprite(AtlasName.StarBuff, "star_on");
        var starOff = ResourcesLoader.Instance.GetSprite(AtlasName.StarBuff, "star_off");

        for (int i = 0; i < count; i++)
        {
            SetImageStar(i, starOn, true);
        }
        for (int i = count; i < stars.Count; i++)
        {
            SetImageStar(i, starOff, false);
        }
    }

    public int GetStarActive()
    {
        return starsActive;
    }
}