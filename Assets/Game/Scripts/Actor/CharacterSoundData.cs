using UnityEngine;

namespace Game.GameActor
{

    //event
    [CreateAssetMenu(menuName = "SoundData/CharacterData")]
    public class CharacterSoundData : ScriptableObject
    {
        public AudioClip[] hurtSFXs, dieSFXs, landSFXs, jumpSFXs, startSFXs, appearSFXs, engageSFXs, getHitSFXs, getHitArmorSFXs, growSFXs;

    }
}