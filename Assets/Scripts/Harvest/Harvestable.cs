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
        [SerializeField] private float cooldownTime = 5f;
        public void SetCooldownTime(float time)
        {
            cooldownTime = time;
        }

        [Tooltip("收获资源类型")]
        [SerializeField] private HarvestType resourceType;

        [Tooltip("每次收获数量")]
        [SerializeField] private int harvestAmount = 1;

        [Header("UI设置")]
        [Tooltip("是否显示进度条")]
        [SerializeField] private bool showProgressBar = true;

        [Tooltip("进度条Y轴偏移量")]
        [SerializeField] private float progressBarYOffset = 0.5f;

        private SquareCell cell;
        private float currentCooldown; // 当前剩余冷却时间
        private bool canHarvest = true;
        private GameObject progressBarObj; // 进度条游戏物体
        private Image fillImage; // 进度条填充图像

        /// <summary>
        /// 在Awake中初始化组件
        /// </summary>
        private void Awake()
        {
            cell = GetComponent<SquareCell>();
            if (cell == null)
            {
                Debug.LogError("Harvestable组件必须附加到带有SquareCell的物体上");
                enabled = false;
                return;
            }

            if (showProgressBar)
            {
                CreateProgressBar();
            }
        }

        /// <summary>
        /// 在组件销毁时清理
        /// </summary>
        private void OnDestroy()
        {
            if (progressBarObj != null)
            {
                Destroy(progressBarObj);
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

                // 更新进度条
                if (fillImage != null)
                {
                    fillImage.fillAmount = 1 - (currentCooldown / cooldownTime);
                }

                if (currentCooldown <= 0)
                {
                    canHarvest = true;

                    // 隐藏进度条
                    if (progressBarObj != null)
                    {
                        progressBarObj.SetActive(false);
                    }
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

            // 显示进度条
            if (progressBarObj != null)
            {
                progressBarObj.SetActive(true);
                fillImage.fillAmount = 0; // 开始时为空
            }

            Debug.Log($"从 {cell.Coordinates} 收获了 {harvestAmount} 个 {resourceType}");
        }

        /// <summary>
        /// 创建简单的世界空间UI进度条
        /// </summary>
        private void CreateProgressBar()
        {
            // 创建进度条容器
            progressBarObj = new GameObject("HarvestProgressBar");
            progressBarObj.transform.SetParent(transform);
            progressBarObj.transform.localPosition = Vector3.up * progressBarYOffset;

            // 添加世界空间Canvas
            Canvas canvas = progressBarObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.worldCamera = Camera.main;

            // 设置Canvas大小
            RectTransform canvasRect = canvas.GetComponent<RectTransform>();
            canvasRect.sizeDelta = new Vector2(1f, 0.2f);
            canvasRect.localScale = new Vector3(0.01f, 0.01f, 1f);

            // 创建背景
            GameObject bgObj = new GameObject("Background");
            bgObj.transform.SetParent(canvasRect);

            RectTransform bgRect = bgObj.AddComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.offsetMin = Vector2.zero;
            bgRect.offsetMax = Vector2.zero;

            Image bgImage = bgObj.AddComponent<Image>();
            bgImage.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);

            // 创建填充
            GameObject fillObj = new GameObject("Fill");
            fillObj.transform.SetParent(bgRect);

            RectTransform fillRect = fillObj.AddComponent<RectTransform>();
            fillRect.anchorMin = Vector2.zero;
            fillRect.anchorMax = Vector2.one;
            fillRect.offsetMin = Vector2.zero;
            fillRect.offsetMax = Vector2.zero;

            fillImage = fillObj.AddComponent<Image>();
            fillImage.color = new Color(0.2f, 0.8f, 0.2f, 1f);
            fillImage.type = Image.Type.Filled;
            fillImage.fillMethod = Image.FillMethod.Horizontal;
            fillImage.fillOrigin = (int)Image.OriginHorizontal.Left;
            fillImage.fillAmount = 0;

            // 添加Billboard组件让UI始终面向摄像机
            progressBarObj.AddComponent<Billboard>();

            // 默认隐藏
            progressBarObj.SetActive(true);
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

