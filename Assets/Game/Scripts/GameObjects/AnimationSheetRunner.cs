using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationSheetRunner : MonoBehaviour
{
    private SpriteRenderer Renderer;
    [SerializeField]
    Sprite[] Sprites;

    [SerializeField]
    float fps = 60;
    float time = 0;
    int index = 0;

    private void OnEnable()
    {
        Renderer = GetComponent<SpriteRenderer>();
    }
    private void Update()
    {
        if (time > 0)
        {
            time -= Time.deltaTime;
        }
        else
        {
            time = 1f / fps;
            Renderer.sprite = Sprites[index % Sprites.Length];
            index++;
        }
    }
}
