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
    
    // 添加物品到库存
    public void AddItem(HarvestType itemType, int amount = 1)
    {
        InventoryItem existingItem = inventory.Find(item => item.itemType == itemType);
        
        if (existingItem != null)
        {
            existingItem.count += amount;
        }
        else
        {
            inventory.Add(new InventoryItem
            {
                itemType = itemType,
                count = amount
            });
        }
        
        // 更新UI显示
        UpdateInventoryUI();
    }
    
    // 使用物品
    public bool UseItem(HarvestType itemType, int amount = 1)
    {
        InventoryItem existingItem = inventory.Find(item => item.itemType == itemType);
        
        if (existingItem != null && existingItem.count >= amount)
        {
            existingItem.count -= amount;
            
            if (existingItem.count <= 0)
                inventory.Remove(existingItem);
                
            // 更新UI显示
            UpdateInventoryUI();
            return true;
        }
        
        return false;
    }
    
    // 获取物品数量
    public int GetItemCount(HarvestType itemType)
    {
        InventoryItem existingItem = inventory.Find(item => item.itemType == itemType);
        return existingItem != null ? existingItem.count : 0;
    }
    
    // 更新库存UI
    private void UpdateInventoryUI()
    {
        // 实现库存UI更新逻辑
    }
}