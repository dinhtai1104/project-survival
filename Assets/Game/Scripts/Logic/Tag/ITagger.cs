using System.Collections.Generic;

/// <summary>
/// Tag object => object is Object|Enemy|Fly|Greate...
/// </summary>
public interface ITagger
{
	bool HasTag(ETag tag);
	void AddTag(ETag tag);
	void AddTags(IEnumerable<ETag> tags);
	bool HasTags(IEnumerable<ETag> tags);
	bool HasTags(params ETag[] tags);
	bool HasAnyTags(IEnumerable<ETag> tags);
    bool HasAnyTags(params ETag[] tags);
    void RemoveTag(ETag tag);
}