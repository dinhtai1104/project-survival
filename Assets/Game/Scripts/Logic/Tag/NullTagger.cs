using System.Collections.Generic;

public class NullTagger : ITagger
{
    public void AddTag(ETag tag)
    {
    }

    public void AddTags(IEnumerable<ETag> tags)
    {
    }

    public bool HasAnyTags(IEnumerable<ETag> tags)
    {
        return false;
    }

    public bool HasAnyTags(params ETag[] tags)
    {
        return false;
    }

    public bool HasTag(ETag tag)
    {
        return false;
    }

    public bool HasTags(IEnumerable<ETag> tags)
    {
        return false;
    }

    public bool HasTags(params ETag[] tags)
    {
        return false;
    }

    public void RemoveTag(ETag tag)
    {
    }
}
