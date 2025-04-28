using System.Collections.Generic;
using CDTU.Utils;
using UnityEngine;

public class HarvestManager : Singleton<HarvestManager>
{
    public Dictionary<HarvestType, int> harvestAmounts = new()
    {
        { HarvestType.Branch,0 },
        { HarvestType.Rice,0 },
    };


    public void AddHarvest(HarvestType harvestType, int amount)
    {
        if (harvestAmounts.ContainsKey(harvestType))
        {
            harvestAmounts[harvestType] += amount;
        }
        else
        {
            Debug.Log("已经自动添加进字典");
            harvestAmounts[harvestType] = amount;
        }
    }

    public void ReduceHarvest(HarvestType harvestType, int amount)
    {
        if (HasHarvest(harvestType) && harvestAmounts[harvestType] >= 0)
        {
            harvestAmounts[harvestType] -= amount;
            if (harvestAmounts[harvestType] <= 0)
            {
                harvestAmounts.Remove(harvestType);
            }
        }

    }

    public bool HasHarvest(HarvestType harvestType)
    {
        if (harvestAmounts.ContainsKey(harvestType))
        {
            return harvestAmounts[harvestType] >= 0;
        }
        return false;
    }







}