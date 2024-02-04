using UnityEngine;

namespace Game.GameActor
{
    public class CollectableItemHandler : MonoBehaviour
    {
        protected Character character;

        public void SetUp(Character character)
        {
            this.character = character;
        }
    }
}