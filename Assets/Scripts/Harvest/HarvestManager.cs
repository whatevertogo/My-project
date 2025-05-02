using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.Events;
using CDTU.Utils;

namespace HexGame.Harvest
{
    /// <summary>
    /// 收获管理器，负责管理所有资源的收获
    /// </summary>
    public class HarvestManager : Singleton<HarvestManager>
    {
        [System.Serializable]
        public class HarvestEvent : UnityEvent<HarvestType, int> { }


        // 收获事件，当资源变化时触发 - 支持C#事件方式
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

        // 存储各类资源的数量
        private readonly Dictionary<HarvestType, int> resourceCounts = new Dictionary<HarvestType, int>();        protected override void Awake()
        {
            base.Awake();
            InitializeResourceCounts();
        }

        private void Start()
        {
            // 初始化时触发事件，通知 UI 当前资源状态
            foreach (HarvestType type in Enum.GetValues(typeof(HarvestType)))
            {
                if (type != HarvestType.None)
                {
                    TriggerHarvestChangedEvent(type, resourceCounts[type]);
                }
            }
        }

        private void InitializeResourceCounts()
        {
            // 初始化所有资源类型的计数为0
            foreach (HarvestType type in Enum.GetValues(typeof(HarvestType)))
            {
                if (type != HarvestType.None)
                {
                    resourceCounts[type] = 0;
                }
            }
        }        /// <summary>
        /// 增加指定类型的资源
        /// </summary>
        /// <param name="type">资源类型</param>
        /// <param name="amount">增加数量</param>
        public void AddHarvest(HarvestType type, int amount)
        {
            if (type == HarvestType.None) return;

            if (amount <= 0)
            {
                Debug.LogWarning("增加的资源数量必须大于 0！");
                return;
            }

            if (!resourceCounts.ContainsKey(type))
            {
                resourceCounts[type] = 0;
            }

            resourceCounts[type] += amount;
            
            TriggerHarvestChangedEvent(type, resourceCounts[type]);

            Debug.Log($"收获了 {amount} 个 {type}，当前总数：{resourceCounts[type]}");
        }

        /// <summary>
        /// 获取指定类型资源的数量
        /// </summary>
        public int GetResourceCount(HarvestType type)
        {
            return resourceCounts.ContainsKey(type) ? resourceCounts[type] : 0;
        }

        /// <summary>
        /// 获取指定类型资源的数量 (别名方法，保持API兼容性)
        /// </summary>
        public int GetHarvestCount(HarvestType type)
        {
            return GetResourceCount(type);
        }

        /// <summary>
        /// 消耗指定类型的资源
        /// </summary>
        /// <returns>是否消耗成功</returns>
        public bool ConsumeResource(HarvestType type, int amount)
        {
            if (type == HarvestType.None) return false;
            
            if (amount <= 0)
            {
                Debug.LogWarning("减少的资源数量必须大于 0！");
                return false;
            }

            if (!resourceCounts.ContainsKey(type) || resourceCounts[type] < amount)
            {
                Debug.LogWarning($"无法消耗 {amount} 个 {type}，当前数量不足！");
                return false;
            }

            resourceCounts[type] -= amount;
            TriggerHarvestChangedEvent(type, resourceCounts[type]);
            return true;
        }
        
        /// <summary>
        /// 减少指定类型的资源 (别名方法，保持API兼容性)
        /// </summary>
        public void ReduceHarvest(HarvestType type, int amount)
        {
            ConsumeResource(type, amount);
        }
        
        /// <summary>
        /// 检查是否拥有指定数量的资源
        /// </summary>
        public bool HasHarvest(HarvestType type, int amount = 1)
        {
            return resourceCounts.TryGetValue(type, out int count) && count >= amount;
        }
        
        /// <summary>
        /// 触发资源变化事件
        /// </summary>
        private void TriggerHarvestChangedEvent(HarvestType type, int amount)
        {
            OnHarvestChanged?.Invoke(this, new OnHarvestChangedEventArgs(type, amount));
        }
    }
}
