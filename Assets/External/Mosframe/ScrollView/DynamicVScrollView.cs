/*
 * DynamicVScrollView.cs
 * 
 * @author mosframe / https://github.com/mosframe
 * 
 */

 namespace Mosframe {
    using DG.Tweening;
    using System;
    using System.Linq;
    using UnityEngine;

    /// <summary>
    /// Dynamic Vertical Scroll View
    /// </summary>
    [AddComponentMenu("UI/Dynamic V Scroll View")]
    public class DynamicVScrollView : DynamicScrollView {
        public float ViewportSize => viewportSize;
        public float ContentSize => contentSize;
        protected override float contentAnchoredPosition    { get { return -this.contentRect.anchoredPosition.y; } set { this.contentRect.anchoredPosition = new Vector2( this.contentRect.anchoredPosition.x, -value ); } }
	    protected override float contentSize                { get { return this.contentRect.rect.height; } }
	    protected override float viewportSize               { get { return this.viewportRect.rect.height;} }
	    protected override float itemSize                   { get { return this.itemPrototype.rect.height;} }

        public override void init () {

            this.direction = Direction.Vertical;
            base.init();
        }
        protected override void Awake() {

            base.Awake();
            this.direction = Direction.Vertical;
        }
        protected override void Start () {

            base.Start();
        }

        [SerializeField] private float Bottom = 0;

        protected override Vector2 getSizeContent()
        {
            return base.getSizeContent() + new Vector2(0, Bottom);
        }

        public void SetPosition(int y)
        {
            contentRect.anchoredPosition = new Vector3(0, y, 0);
        }

        public void SetActiveFalseAllExcept(params int[] targetIndex)
        {
            foreach (var itemObj in this.containers)
            {
                var item = itemObj.GetComponent<IScrollerDynamicScrollView>();
                if (item == null) continue;
                var index = item.getIndex();
                if (!targetIndex.Contains(index))
                {
                    item.Active(false);
                }
                else
                {
                    item.Active(true);
                }
            }
        }
    }
}
