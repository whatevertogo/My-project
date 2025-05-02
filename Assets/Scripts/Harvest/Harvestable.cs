using UnityEngine;
using System.Collections;

namespace HexGame.Harvest
{
    /// <summary>
    /// 可收获组件，用于处理格子的资源收获逻辑
    /// </summary>
    public class Harvestable : MonoBehaviour
    {
        [Header("收获设置")]
        [Tooltip("收获冷却时间（秒）")]
        [SerializeField] private float cooldownTime = 5f;

        [Tooltip("收获资源类型")]
        [SerializeField] private HarvestType resourceType;

        [Tooltip("每次收获数量")]
        [SerializeField] private int harvestAmount = 1;

        private SquareCell cell;
        private float lastHarvestTime;
        private bool canHarvest = true;

        private void Awake()
        {
            cell = GetComponent<SquareCell>();
            if (cell == null)
            {
                Debug.LogError("Harvestable组件必须附加到带有SquareCell的物体上");
                enabled = false;
                return;
            }
        }

        private void OnMouseDown()
        {
            if (canHarvest && Time.time >= lastHarvestTime + cooldownTime)
            {
                Harvest();
            }
        }

        /// <summary>
        /// 执行收获操作
        /// </summary>
        public void Harvest()
        {
            // 执行收获
            HarvestManager.Instance.AddHarvest(resourceType, harvestAmount);

            // 记录收获时间
            lastHarvestTime = Time.time;

            // 开始冷却
            StartCoroutine(StartCooldown());

            // 添加视觉反馈
            // TODO: 添加粒子效果或其他视觉反馈

            Debug.Log($"从 {cell.Coordinates} 收获了 {harvestAmount} 个 {resourceType}");
        }

        private IEnumerator StartCooldown()
        {
            canHarvest = false;

            // TODO: 更新视觉效果来显示冷却状态

            yield return new WaitForSeconds(cooldownTime);

            canHarvest = true;

            // TODO: 更新视觉效果来显示可收获状态
        }

        /// <summary>
        /// 设置收获资源类型
        /// </summary>
        public void SetResourceType(HarvestType type)
        {
            resourceType = type;
        }

        /// <summary>
        /// 设置收获数量
        /// </summary>
        public void SetHarvestAmount(int amount)
        {
            harvestAmount = amount;
        }

        /// <summary>
        /// 检查是否可以收获
        /// </summary>
        public bool CanHarvest()
        {
            return canHarvest && Time.time >= lastHarvestTime + cooldownTime;
        }
    }
}
