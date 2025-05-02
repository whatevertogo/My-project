using System;
using System.Collections.Generic;
using CDTU.Utils;
using HexGame.Harvest;
using UnityEngine;

/// <summary>
/// 管理收获资源的单例类
/// </summary>
public class HarvestManager : Singleton<HarvestManager>
{
    /// <summary>
    /// 当资源数量发生变化时触发的事件，用于更新 UI 等逻辑
    /// </summary>
    public event EventHandler<OnHarvestChangedEventArgs> OnHarvestChanged;

    /// <summary>
    /// 资源变化事件参数
    /// </summary>
    public class OnHarvestChangedEventArgs : EventArgs
    {
        public HarvestType HarvestType { get; }
        public int Amount { get; }

        public OnHarvestChangedEventArgs(HarvestType harvestType, int amount)
        {
            HarvestType = harvestType;
            Amount = amount;
        }
    }

    /// <summary>
    /// 存储每种资源的数量
    /// </summary>
    private Dictionary<HarvestType, int> harvestAmounts = new()
    {
        { HarvestType.Branch, 0 },
        { HarvestType.PineCone, 0 },
        { HarvestType.Feather, 0 }
    };

    private void Start()
    {
        // 初始化时触发事件，通知 UI 当前资源状态
        TriggerHarvestChangedEvent(HarvestType.Branch, 0);
        TriggerHarvestChangedEvent(HarvestType.PineCone, 0);
        //TODO- 其他资源的初始化
    }

    /// <summary>
    /// 增加指定类型的资源数量
    /// </summary>
    /// <param name="harvestType">资源类型</param>
    /// <param name="amount">增加的数量</param>
    public void AddHarvest(HarvestType harvestType, int amount)
    {
        if (amount <= 0)
        {
            Debug.LogWarning("增加的资源数量必须大于 0！");
            return;
        }

        if (harvestAmounts.TryGetValue(harvestType, out int currentAmount))
        {
            harvestAmounts[harvestType] = currentAmount + amount;
        }
        else
        {
            Debug.Log($"资源类型 {harvestType} 不存在，已自动添加到字典中。");
            harvestAmounts[harvestType] = amount;
        }

        Debug.Log($"增加了 {amount} 个 {harvestType}，当前数量为：{harvestAmounts[harvestType]}");
        TriggerHarvestChangedEvent(harvestType, harvestAmounts[harvestType]);
    }

    /// <summary>
    /// 减少指定类型的资源数量
    /// </summary>
    /// <param name="harvestType">资源类型</param>
    /// <param name="amount">减少的数量</param>
    public void ReduceHarvest(HarvestType harvestType, int amount)
    {
        if (amount <= 0)
        {
            Debug.LogWarning("减少的资源数量必须大于 0！");
            return;
        }

        if (harvestAmounts.TryGetValue(harvestType, out int currentAmount))
        {
            if (currentAmount >= amount)
            {
                harvestAmounts[harvestType] -= amount;
                Debug.Log($"减少了 {amount} 个 {harvestType}，当前数量为：{harvestAmounts[harvestType]}");
            }
            else
            {
                Debug.LogWarning($"无法减少 {amount} 个 {harvestType}，当前数量不足！");
                //todo-失败结局
                return;
            }
        }
        else
        {
            Debug.LogWarning($"资源类型 {harvestType} 不存在，无法减少！");
            return;
        }

        TriggerHarvestChangedEvent(harvestType, harvestAmounts[harvestType]);
    }

    /// <summary>
    /// 检查是否拥有指定类型的资源
    /// </summary>
    /// <param name="harvestType">资源类型</param>
    /// <returns>是否拥有该资源</returns>
    public bool HasHarvest(HarvestType harvestType)
    {
        return harvestAmounts.TryGetValue(harvestType, out int amount) && amount > 0;
    }

    /// <summary>
    /// 获取指定类型资源的数量
    /// </summary>
    /// <param name="harvestType">资源类型</param>
    /// <returns>资源数量</returns>
    public int GetHarvestCount(HarvestType harvestType)
    {
        if (harvestAmounts.TryGetValue(harvestType, out int count))
        {
            return count;
        }

        Debug.LogWarning($"资源类型 {harvestType} 不存在！");
        return 0;
    }

    /// <summary>
    /// 触发资源变化事件
    /// </summary>
    /// <param name="harvestType">资源类型</param>
    /// <param name="amount">当前数量</param>
    private void TriggerHarvestChangedEvent(HarvestType harvestType, int amount)
    {
        OnHarvestChanged?.Invoke(this, new OnHarvestChangedEventArgs(harvestType, amount));
    }
}