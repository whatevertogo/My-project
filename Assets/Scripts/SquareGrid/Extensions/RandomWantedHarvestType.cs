using System;
using System.Linq;
using HexGame.Harvest;

public static class RandomWantedHarvestType
{
    private static readonly HarvestType[] values = (HarvestType[])Enum.GetValues(typeof(HarvestType));

    public static HarvestType GetRandomHarvestType()
    {
        return values[UnityEngine.Random.Range(0, values.Length)];
    }
}