using System.Collections.Generic;
using CDTU.Utils;
using UnityEngine;

public class InventoryManager : Singleton<InventoryManager>
{
    [System.Serializable]
    public class InventoryItem
    {
        public HarvestType itemType;
        public int count;
    }

    public List<InventoryItem> inventory = new List<InventoryItem>();

    public Dictionary<HarvestType, int> harvestAmounts = new();

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

    // 获取物品数量
    public int GetItemCount(HarvestType itemType)
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

    // 更新库存UI
    private void UpdateInventoryUI()
    {
        
        // 实现库存UI更新逻辑
    }
}