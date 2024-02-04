using Assets.Game.Scripts.BaseFramework.Architecture;
using Assets.Game.Scripts.Talent.Database;
using com.mec;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Mosframe;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Game.Scripts.Talent.UI.Upgrade
{
    public class UITalentUpgradeViewItem : MonoBehaviour
    {
        [SerializeField] private DynamicVScrollView scrollView;
        [SerializeField] private UITalentUpgradeInfor inforTalent;
        [SerializeField] private Button button;
        private UITalentUpgradeBoard _board;
        private List<int> IndexInScroll = new List<int>();
        private TalentEntity target;
        public async UniTask Run(UITalentUpgradeBoard board, TalentService _service, TalentEntity target, float duration, int itemFake, System.Action onStop = null)
        {
            this.target = target;
            this._board = board;
            button.interactable = false;
            inforTalent.Active(false);
            IndexInScroll.Clear();

            int itemCount = DataManager.Base.Talent.Dictionary.Count;
            IndexInScroll.Add(Random.Range(0, itemCount - 1));
            IndexInScroll.Add(target.Id);

            for (int i = 0; i < itemFake; i++)
            {
                IndexInScroll.Add(Random.Range(0, itemCount - 1));
            }
            scrollView.init(itemFake + 2);
            await DelayRun(0.3f);
            async UniTask DelayRun(float delay)
            {
                await UniTask.Yield();
                scrollView.scrollToLastPos();
                scrollView.refresh();
                await UniTask.Delay(System.TimeSpan.FromSeconds(delay));
                await scrollView.scroll(1, duration, null, ease: Ease.OutExpo).AsyncWaitForCompletion();
                
                scrollView.scrollByItemIndex(1);
                inforTalent.Set(target, _service.GetLevel(target.Id));
                button.interactable = true;
            }
        }

        public TalentEntity GetEntity(int index)
        {
            return DataManager.Base.Talent.GetTalentByIndex(IndexInScroll[index]);
        }

        public void PickThisTalentOnClicked()
        {
            _board.DisactiveAllButton();
            _board.PickThis(target);
        }
    }
}
