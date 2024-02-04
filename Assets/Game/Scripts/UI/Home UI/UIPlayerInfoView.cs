using Assets.Game.Scripts.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayerInfoView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI playerNameTxt;
    [SerializeField] private TextMeshProUGUI playerLevelTxt;
    [SerializeField] private Image playerAvatarImg;
    [SerializeField] private Image playerProgressLevelImg;

    private UserSave userSave;
    private ExpHandler expHandler;

    private void Awake()
    {
        expHandler = GameSceneManager.Instance.PlayerData.ExpHandler;
    }

    private void OnEnable()
    {
        expHandler.OnLevelChanged += OnLevelChanged;
        expHandler.OnExpChanged += OnExpChanged;
        Messenger.AddListener<EHero>(EventKey.PickHero, OnPickHero);

        Setup();
    }
    private void OnDisable()
    {
        expHandler.OnLevelChanged -= OnLevelChanged;
        expHandler.OnExpChanged -= OnExpChanged;
        Messenger.RemoveListener<EHero>(EventKey.PickHero, OnPickHero);
    }

    private void OnPickHero(EHero hero)
    {
        Setup();
    }

    private void OnExpChanged(long exp)
    {
        Setup();
    }

    private void OnLevelChanged(int from, int to)
    {
        Setup();
    }

    public void Setup()
    {
        userSave = DataManager.Save.User;

        SetPlayerName();
        SetPlayerAvatar();
        SetLevelPlayer();
        SetProgressLevel();
    }

    private void SetLevelPlayer()
    {
        playerLevelTxt.text = expHandler.CurrentLevel.ToString();
    }

    private void SetPlayerAvatar()
    {
        playerAvatarImg.sprite = ResourcesLoader.Instance.GetSprite(AtlasName.Hero, userSave.Hero.ToString());
    }
    private void SetPlayerName()
    {
        playerNameTxt.text = userSave.PlayerName;
    }
    private void SetProgressLevel()
    {
        var progressLevel = expHandler.LevelProgress;
        playerProgressLevelImg.fillAmount = progressLevel;
    }
}
