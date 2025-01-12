using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeightFunctions
{
    public static int GetRandomWeightedIndex(int[] weights)
    {
        if (weights == null || weights.Length == 0) return -1;

        int weightSum = 0;
        int i;
        for (i = 0; i < weights.Length; i++)
        {
            if (weights[i] >= 0) weightSum += weights[i];
        }

        float r = Random.value;
        float s = 0f;

        for (i = 0; i < weights.Length; i++)
        {
            if (weights[i] <= 0f) continue;

            s += (float)weights[i] / weightSum;
            if (s >= r) return i;
        }

        return -1;
    }
}
