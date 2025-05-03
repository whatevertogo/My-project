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
        private float cooldownTime = 10f;
        [Tooltip("生长时间（秒）")]
        private float growthTime = 10f; // 生长时间

        [Header("动画设置")]
        [Tooltip("树木的Animator组件")]
        private Animator treeAnimator;
        [SerializeField] private float growthDuration = 10f; // 总生长时间

        public void SetCooldownTime(float time)
        {
            cooldownTime = time;
        }

        [Tooltip("收获资源类型")]
        [SerializeField] private HarvestType resourceType;

        [Tooltip("每次收获数量")]
        public int harvestAmount = 1;

        private SpriteRenderer pineConeRenderer;

        private Sprite[] pineSprites;

        private SquareCell cell;
        private float currentCooldown; // 当前剩余冷却时间
        private bool canHarvest = true;
        private bool isGrown = true;

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
        private void Start()
        {
            if (resourceType == HarvestType.PineCone)
            {
                isGrown = false;
                // 获取CellPineCone子物体的SpriteRenderer
                var pineObj = transform.Find("CellPineCone");
                if (pineObj == null)
                {
                    Debug.LogError("找不到CellPineCone子物体");
                    return;
                }
                
                pineConeRenderer = pineObj.GetComponent<SpriteRenderer>();
                if (pineConeRenderer == null)
                {
                    Debug.LogError("CellPineCone上找不到SpriteRenderer组件");
                    return;
                }

                // 预加载两张生长阶段图片
                pineSprites = new Sprite[2];
                pineSprites[0] = Resources.Load<Sprite>("Images/Tree/huaShu2");
                pineSprites[1] = Resources.Load<Sprite>("Images/Tree/huaShu1");

                // 检查图片是否加载成功
                for (int i = 0; i < pineSprites.Length; i++)
                {
                    if (pineSprites[i] == null)
                    {
                        Debug.LogError($"无法加载图片: Images/Tree/huaShu{2-i}");
                    }
                }

                if (pineConeRenderer != null && pineSprites[0] != null)
                {
                    pineConeRenderer.sprite = pineSprites[0];
                }
            }
        }

        /// <summary>
        /// 鼠标点击事件，用于收获资源
        /// </summary>
        public void OnMouseDown()
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
            if (resourceType == HarvestType.PineCone && pineConeRenderer && pineSprites != null)
            {
                float progress = 1f - Mathf.Clamp01(growthTime / growthDuration);
                if (progress < 1f / 2f) // 只分两个阶段
                {
                    pineConeRenderer.sprite = pineSprites[0]; // huaShu2
                }
                else
                {
                    pineConeRenderer.sprite = pineSprites[1]; // huaShu1
                }
            }

            if (!isGrown)
            {
                growthTime -= Time.deltaTime;
            }

            // 处理冷却时间
            if (!canHarvest && currentCooldown > 0)
            {
                currentCooldown -= Time.deltaTime;
                // 当冷却时间结束时，重置可收获状态
                if (currentCooldown <= 0)
                {
                    canHarvest = true;
                    currentCooldown = 0;
                }
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

}

