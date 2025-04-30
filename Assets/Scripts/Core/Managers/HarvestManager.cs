using System;
using System.Collections.Generic;
using CDTU.Utils;
using UnityEngine;

public class HarvestManager : Singleton<HarvestManager>
{

    public event EventHandler<OnHarvestChangedEventArgs> OnHarvestChanged;
    //通过事件更新UI

    public class OnHarvestChangedEventArgs : EventArgs
    {
        public HarvestType harvestType;
        public int amount;

        public OnHarvestChangedEventArgs(HarvestType harvestType, int amount)
        {
            this.harvestType = harvestType;
            this.amount = amount;
        }
    }

    public Dictionary<HarvestType, int> harvestAmounts = new()
    {
        { HarvestType.Branch,0 },
        { HarvestType.PineCone,0 },
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
        OnHarvestChanged.Invoke(this, new OnHarvestChangedEventArgs(harvestType, harvestAmounts[harvestType]));
    }

    public void ReduceHarvest(HarvestType harvestType, int amount)
    {
        if (HasHarvest(harvestType) && harvestAmounts[harvestType] >= 0)
        {
            harvestAmounts[harvestType] -= amount;
            Debug.Log($"减少了{harvestType}的数量，当前数量为：{harvestAmounts[harvestType]}");
        }
        OnHarvestChanged.Invoke(this, new OnHarvestChangedEventArgs(harvestType, harvestAmounts[harvestType]));
    }

    public bool HasHarvest(HarvestType harvestType)
    {
        if (harvestAmounts.ContainsKey(harvestType))
        {
            return harvestAmounts[harvestType] >= 0;
        }
        return false;
    }

    public int GetHarvestCount(HarvestType itemType)
    {
        if (harvestAmounts.TryGetValue(itemType, out int count))
        {
            return count;
        }
        else
        {
            Debug.LogWarning("物品类型不存在于库存中！");
            return 0;
        }
    }


}