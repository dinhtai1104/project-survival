using System.Collections.Generic;
using UnityEngine;

public class Tagger : MonoBehaviour, ITagger
{
    public List<ETag> tags = new List<ETag>();

    public void AddTag(ETag tag)
    {
        if (HasTag(tag)) return;
        tags.Add(tag);
    }

    public void AddTags(IEnumerable<ETag> tags)
    {
        foreach (ETag tag in tags)
        {
            AddTag(tag);
        }
    }

    public bool HasAnyTags(IEnumerable<ETag> tags)
    {
        foreach (ETag tag in tags)
        {
            if (HasTag(tag)) return true;
        }
        return false;
    }

    public bool HasAnyTags(params ETag[] tags)
    {
        foreach (ETag tag in tags)
        {
            if (HasTag(tag)) return true;
        }
        return false;
    }

    public bool HasTag(ETag tag)
    {
        return tags.Contains(tag);
    }

    public bool HasTags(IEnumerable<ETag> tags)
    {
        foreach (ETag tag in tags)
        {
            if (!HasTag(tag)) return false;
        }
        return true;
    }

    public bool HasTags(params ETag[] tags)
    {
        foreach (ETag tag in tags)
        {
            if (!HasTag(tag)) return false;
        }
        return true;
    }

    public void RemoveTag(ETag tag)
    {
        if (tags.Contains(tag))
        {
            tags.Remove(tag);
        }
    }
}