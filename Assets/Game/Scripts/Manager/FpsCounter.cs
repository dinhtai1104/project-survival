using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FpsCounter : MonoBehaviour
{
#if DEVELOPMENT
    // for ui. 
    private int screenLongSide;
    private Rect boxRect;
    private GUIStyle style = new GUIStyle();

    // for fps calculation.
    private int frameCount;
    private float elapsedTime;
    private double frameRate;
    private bool isRunning;
    private int killEnemy = 0;

    /// <summary>
    /// Initialization
    /// </summary>
    private void Awake()
    {
        UpdateUISize();
    }

    private void Start()
    {
    }

    private void OnDestroy()
    {
    }

    void OnEnemyDie(object obj)
    {
        killEnemy++;
    }

    private void OnRunGame(object obj)
    {
        isRunning = true;
    }

    private void OnWinGame(object obj)
    {
        isRunning = false;
    }

    /// <summary>
    /// Monitor changes in resolution and calcurate FPS
    /// </summary>
    private void Update()
    {
        // FPS calculation
        frameCount++;
        elapsedTime += Time.deltaTime;
        if (elapsedTime > 0.5f)
        {
            frameRate = System.Math.Round(frameCount / elapsedTime, 1, System.MidpointRounding.AwayFromZero);
            frameCount = 0;
            elapsedTime = 0;

            // Update the UI size if the resolution has changed
            if (screenLongSide != Mathf.Max(Screen.width, Screen.height))
            {
                UpdateUISize();
            }
        }
    }

    /// <summary>
    /// Resize the UI according to the screen resolution
    /// </summary>
    private void UpdateUISize()
    {
        screenLongSide = Mathf.Max(Screen.width, Screen.height);
        var rectLongSide = screenLongSide / 8f;
        boxRect = new Rect(1f, 1f, rectLongSide, rectLongSide / 4f);
        style.fontSize = (int)(screenLongSide / 40);
        style.normal.textColor = Color.green;
        style.alignment = TextAnchor.MiddleCenter;
    }

    /// <summary>
    /// Display FPS
    /// </summary>
    private void OnGUI()
    {
        GUI.Box(boxRect, "");
        GUI.Label(boxRect, " " + Mathf.Round((float)frameRate) + " fps", style);
    }
#endif
}