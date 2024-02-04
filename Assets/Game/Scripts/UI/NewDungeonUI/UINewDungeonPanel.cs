using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class UINewDungeonPanel : UI.Panel
{
    [SerializeField] private UIMapItem map;
    [SerializeField] private Image[] boss;
    [SerializeField] private RectTransform[] bossPos;
    [SerializeField] private Transform buffCardHolder;
    [SerializeField] private UIBuffItemBase buffPrefab;

    bool canClose = false;
    private int DungeonId;
    public override void PostInit()
    {
    }

    [Button]
    public async void Show(int DungeonId)
    {
        ShowBuffCard(DungeonId);
        base.Show();
        this.DungeonId = DungeonId;
        var menu = PanelManager.Instance.GetPanel<UIMainMenuPanel>();
        var currentMap = menu.MainMenuGate.GetCurrentMap();
        var worldMap = DataManager.Base.DungeonWorld.Get(DungeonId);
        map.Setup(DungeonId);
        menu.fadeBlackScreenMap.DOFade(1, 0.2f);

        for (int i = 0; i < boss.Length; i++)
        {
            currentMap.MapBossVisuals[i].gameObject.SetActive(false);
        }

        var list = new List<UniTask>();
        for (int i = 0; i < worldMap.Stages.Count; i++)
        {
            var path = $"Boss/UIBoss_{worldMap.Dungeon + 1}_{i + 1}.prefab";
            var pos = bossPos[i];
            var task = ResourcesLoader.Instance.GetGOAsync(path, pos);
            list.Add(task);
        }
        onClosed = OnClosed;

        await UniTask.Delay(1000);
        canClose = true;
    }

    private void ShowBuffCard(int dungeon)
    {
        var unlockBuffs = DataManager.Base.Buff.BuffUnlockAtDungeon(dungeon);
        if (unlockBuffs.Count == 0)
        {
            buffCardHolder.gameObject.SetActive(false);
            return;
        }
        else
        {
            buffCardHolder.gameObject.SetActive(true);
        }
        foreach (var buff in unlockBuffs)
        {
            var bufIns = PoolManager.Instance.Spawn(buffPrefab, buffCardHolder);
            bufIns.SetEntity(buff);
            bufIns.SetInfor();
        }
    }

    public override void Close()
    {
        if (!canClose) return;
        base.Close();
    }
    private void OnClosed()
    {
        var menu = PanelManager.Instance.GetPanel<UIMainMenuPanel>();
        var currentMap = menu.MainMenuGate.GetCurrentMap();
        var worldMap = DataManager.Base.DungeonWorld.Get(DungeonId);

        menu.SetArea(menu.MainMenuGate.gameObject, EArea.Front);


        for (int i = 0; i < boss.Length; i++)
        {
            var pos = boss[i].rectTransform.position;
            var target = currentMap.MapBossVisuals[i];

            var icon = ResourcesLoader.Instance.GetSprite("AvatarBoss", $"boss_{worldMap.Stages[i].Index + 1}_map_{worldMap.Dungeon + 1}");
            int index = i;

            UniTask.Delay(TimeSpan.FromSeconds(index * 0.3f + 0.2f)).ContinueWith(() =>
            {
                ResourcesLoader.Instance.GetAsync<ParticleSystem>($"VFX_UIFloat_Icon_MiniBoss_{index}", PanelManager.Instance.transform).ContinueWith(async effect =>
                {
                    (effect.transform as RectTransform).position = (target.transform as RectTransform).position;
                    effect.Play();
                    await UniTask.Delay(TimeSpan.FromSeconds(0.5f));
                    target.gameObject.SetActive(true);
                    target.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack).From(Vector3.zero);
                    Show();
                    //sfx_new_boss_fly
                }).Forget();

                async UniTask Show()
                {
                    var audio = ResourcesLoader.Instance.Load<AudioClip>(AddressableName.SFX_UI + "sfx_new_boss_fly.ogg");
                    Sound.Controller.Instance.PlayOneShot(audio, 1);
                    await UniTask.Delay(TimeSpan.FromSeconds(audio.length));
                    //ResourcesLoader.Instance.UnloadAsset<AudioClip>(AddressableName.SFX_UI + "sfx_new_boss_fly.ogg");
                }
            }).Forget();
        }
        menu.fadeBlackScreenMap.DOFade(0, 0.4f).SetDelay(2f).OnComplete(() =>
        {
            menu.SetArea(menu.MainMenuGate.gameObject, EArea.Behind);
        });

    }
}
