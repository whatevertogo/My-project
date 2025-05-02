using UnityEngine;
using UnityEngine.UI;

namespace HexGame.Harvest
{
    /// <summary>
    /// 可收获组件，用于处理格子的资源收获逻辑
    /// </summary>
    public class Harvestable : MonoBehaviour
    {
        [Header("收获设置")]
        [Tooltip("收获冷却时间（秒）")]
        [SerializeField] private float cooldownTime = 10f;
        public void SetCooldownTime(float time)
        {
            cooldownTime = time;
        }

        [Tooltip("收获资源类型")]
        [SerializeField] private HarvestType resourceType;

        [Tooltip("每次收获数量")]
        [SerializeField] private int harvestAmount = 1;

        private SquareCell cell;
        private float currentCooldown; // 当前剩余冷却时间
        private bool canHarvest = true;

        /// <summary>
        /// 在Awake中初始化组件
        /// </summary>
        private void Awake()
        {
            cell = GetComponent<SquareCell>();
            if (cell is null)
            {
                Debug.LogError("Harvestable组件必须附加到带有SquareCell的物体上");
                enabled = false;
                return;
            }
        }

        /// <summary>
        /// 鼠标点击事件，用于收获资源
        /// </summary>
        private void OnMouseDown()
        {
            if (canHarvest && currentCooldown <= 0)
            {
                Harvest();
            }
        }

        /// <summary>
        /// 每帧更新冷却时间和UI
        /// </summary>
        private void Update()
        {
            if (!canHarvest && currentCooldown > 0)
            {
                // 使用 Time.deltaTime，这样在游戏暂停时（TimeScale = 0）就不会继续倒计时
                currentCooldown -= Time.deltaTime;
            
            }
        }

        /// <summary>
        /// 执行收获操作
        /// </summary>
        public void Harvest()
        {
            if (!canHarvest) return;

            // 执行收获
            HarvestManager.Instance.AddHarvest(resourceType, harvestAmount);

            // 开始冷却
            canHarvest = false;
            currentCooldown = cooldownTime;

            Debug.Log($"从 {cell.Coordinates} 收获了 {harvestAmount} 个 {resourceType}");
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
            return canHarvest && currentCooldown <= 0;
        }

        /// <summary>
        /// 获取当前冷却进度（0-1）
        /// </summary>
        public float GetCooldownProgress()
        {
            if (canHarvest) return 0;
            return currentCooldown / cooldownTime;
        }
    }

    /// <summary>
    /// 让UI始终面向摄像机的组件
    /// </summary>
    public class Billboard : MonoBehaviour
    {
        private void LateUpdate()
        {
            if (Camera.main is not null)
            {
                transform.forward = Camera.main.transform.forward;
            }
        }
    }
}

