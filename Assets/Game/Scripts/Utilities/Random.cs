using System.Collections.Generic;

namespace GameUtility
{
    public static class Random
    {
        public static float Range(RangeValue rangeValue)
        {
            return UnityEngine.Random.Range(rangeValue.min, rangeValue.max + 1);
        }
        public static int RandomWeighted<T>(this IList<T> weightables) where T : IWeightable
        {
            // Calculate sum of chance
            var sumChance = 0f;

            foreach (var weightable in weightables)
            {
                sumChance += weightable.Weight;
            }

            var rand = UnityEngine.Random.Range(0, sumChance);
            var minRange = 0f;
            var maxRange = 0f;
            var result = 0;

            for (int index = 0; index < weightables.Count; ++index)
            {
                if (weightables[index].Weight <= 0f) continue;

                minRange = maxRange;
                maxRange = minRange + weightables[index].Weight;

                if (rand >= minRange && rand <= maxRange)
                {
                    result = index;
                    break;
                }
            }
            return result;
        }
        public static T RandomWeighted<T>(this IList<T> weightables, out int Index) where T : IWeightable
        {
            // Calculate sum of chance
            var sumChance = 0f;

            foreach (var weightable in weightables)
            {
                sumChance += weightable.Weight;
            }

            var rand = UnityEngine.Random.Range(0, sumChance);
            var minRange = 0f;
            var maxRange = 0f;
            var result = 0;

            for (int index = 0; index < weightables.Count; ++index)
            {
                if (weightables[index].Weight <= 0f) continue;

                minRange = maxRange;
                maxRange = minRange + weightables[index].Weight;

                if (rand >= minRange && rand <= maxRange)
                {
                    result = index;
                    break;
                }
            }
            Index = result;
            if (Index >= 0 && Index < weightables.Count)
            {
                return weightables[Index];
            }
            return weightables[0];
        }
        public static int RandomIndexWeighted(IList<int> chances)
        {
            // Calculate sum of chance
            var sumChance = 0f;

            for (var i = 0; i < chances.Count; i++)
            {
                sumChance += chances[i];
            }

            var rand = UnityEngine.Random.Range(0f, sumChance);
            var minRange = 0f;
            var maxRange = 0f;
            var result = 0;

            for (int i = 0; i < chances.Count; ++i)
            {
                if (chances[i] <= 0f) continue;

                minRange = maxRange;
                maxRange = minRange + chances[i];

                if (rand >= minRange && rand <= maxRange)
                {
                    result = i;
                    break;
                }
            }

            return result;
        }
        public static int RandomIndexWeighted(IList<float> chances)
        {
            // Calculate sum of chance
            var sumChance = 0f;

            for (var i = 0; i < chances.Count; i++)
            {
                sumChance += chances[i];
            }

            var rand = UnityEngine.Random.Range(0f, sumChance);
            var minRange = 0f;
            var maxRange = 0f;
            var result = 0;

            for (int i = 0; i < chances.Count; ++i)
            {
                if (chances[i] <= 0f) continue;

                minRange = maxRange;
                maxRange = minRange + chances[i];

                if (rand >= minRange && rand <= maxRange)
                {
                    result = i;
                    break;
                }
            }

            return result;
        }
    }
}