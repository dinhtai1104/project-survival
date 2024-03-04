using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Engine
{
    public interface ITagger
    {
        int Id { get; }
        IList<string> Tags { get; }
        void AddTag(string tag);
        void RemoveTag(string tag);
        bool HasTag(string tag);
        bool HasTags(IEnumerable<string> tags);
        bool HasAnyOfTags(IEnumerable<string> tags);
        bool HasAllOfTags(IEnumerable<string> tags);
    }
}