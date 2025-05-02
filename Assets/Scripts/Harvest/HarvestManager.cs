using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

namespace HexGame.Harvest
{
    /// <summary>
    /// 收获管理器，负责管理所有资源的收获
    /// </summary>
    public class HarvestManager : MonoBehaviour
    {
        public static HarvestManager Instance { get; private set; }

        [System.Serializable]
        public class HarvestEvent : UnityEvent<HarvestType, int> { }

        // 收获事件，当收获资源时触发
        public HarvestEvent onHarvest = new HarvestEvent();

        // 存储各类资源的数量
        private Dictionary<HarvestType, int> resourceCounts = new Dictionary<HarvestType, int>();

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeResourceCounts();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void InitializeResourceCounts()
        {
            // 初始化所有资源类型的计数为0
            foreach (HarvestType type in System.Enum.GetValues(typeof(HarvestType)))
            {
                if (type != HarvestType.None)
                {
                    resourceCounts[type] = 0;
                }
            }
        }

        /// <summary>
        /// 增加指定类型的资源
        /// </summary>
        /// <param name="type">资源类型</param>
        /// <param name="amount">增加数量</param>
        public void AddHarvest(HarvestType type, int amount)
        {
            if (type == HarvestType.None) return;

            if (!resourceCounts.ContainsKey(type))
            {
                resourceCounts[type] = 0;
            }

            resourceCounts[type] += amount;
            onHarvest.Invoke(type, amount);

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
        /// 消耗指定类型的资源
        /// </summary>
        /// <returns>是否消耗成功</returns>
        public bool ConsumeResource(HarvestType type, int amount)
        {
            if (!resourceCounts.ContainsKey(type) || resourceCounts[type] < amount)
            {
                return false;
            }

            resourceCounts[type] -= amount;
            return true;
        }
    }
}
