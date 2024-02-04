using Spine.Unity;

public class BoneFollowerCustom : BoneFollower
{
    public string boneString;
    private void OnValidate()
    {
        boneName = boneString;
    }
    private void OnEnable()
    {
        
    }
}