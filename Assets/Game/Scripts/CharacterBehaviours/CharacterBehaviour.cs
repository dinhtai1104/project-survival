using UnityEngine;

namespace Game.GameActor
{
    public class CharacterBehaviour : ScriptableObject
    {
        public bool active = false;
        public virtual CharacterBehaviour SetUp(ActorBase character)
        {
            CharacterBehaviour instance = (CharacterBehaviour)CreateInstance(GetType().ToString());
            instance.name = name;
            return instance;
        }
        public virtual void OnActive(ActorBase character)
        {

        }
        public virtual void OnDeactive(ActorBase character)
        {
            active = false;
        }
        public virtual void OnDead(ActorBase character)
        {

        }
        public virtual void OnGetHit(ActorBase character)
        {

        }
        public virtual void OnUpdate(ActorBase character)
        {

        }
    }
}