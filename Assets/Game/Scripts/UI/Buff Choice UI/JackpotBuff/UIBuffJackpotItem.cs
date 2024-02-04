using Assets.Game.Scripts.Utilities;
using Mosframe;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIBuffJackpotItem : UIBehaviour, IDynamicScrollViewItem
{
    private int m_index = 0;
    public UIBuffItem buffItem;
    [SerializeField] private Image iconImg;
    [SerializeField] private Image bgIcon;
    public int getIndex()
    {
        return m_index;
    }

    public void onUpdateItem(int index)
    {
        if (buffItem.IdDataBuff.Count == 0) return;
        m_index = index;
        if (m_index >= buffItem.IdDataBuff.Count && buffItem.IdDataBuff.Count > 0)
        {
            m_index %= buffItem.IdDataBuff.Count;
        }
        var id = buffItem.IdDataBuff[m_index];
        var buff = BuffExtension.GetBuff(id,  buffItem.buffType);
        if (buff != null)
        {
            iconImg.sprite = ResourcesLoader.Instance.GetSprite(AtlasName.Buff, buff.Icon);
            bgIcon.sprite = ResourcesLoader.Instance.GetSprite(AtlasName.Common, $"buff_border_{(int)buff.Classify + 1}");
        }
    }
}