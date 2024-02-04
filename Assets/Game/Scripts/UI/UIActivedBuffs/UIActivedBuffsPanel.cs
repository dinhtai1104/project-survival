using UnityEngine;
public class UIActivedBuffsPanel : UI.Panel
{
    [SerializeField] private UIBuffCollectionView collectionView;
    public override void PostInit()
    {
        var data = new BuffCollectionData { BuffEquiped = GameController.Instance.GetSession().buffSession.Dungeon.BuffEquiped };
        collectionView.Show(data);
    }
}
