using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.AI
{
    [CreateAssetMenu(fileName = "AI/MainchineBrain")]
    public class MachineBrain : ScriptableObject
    {
        [SerializeField, Header("Always Check")]
        private BrainTransition[] coreBrainTransition;
        [SerializeField, Header("Check if AI enable")]
        private BrainNeuronTransition[] localBrainTransition;

        public BrainTransition[] CoreBrainTransition { get => coreBrainTransition; set => coreBrainTransition = value; }
        public BrainNeuronTransition[] LocalBrainTransition { get => localBrainTransition; set => localBrainTransition = value; }
    }
}