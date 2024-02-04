using System.Collections.Generic;

public interface IRewardChestService
{
    void Initialize(List<LootParams> data, LootParams target);
    void Play();
}