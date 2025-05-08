using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

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

        [Header("动画设置")]
        [SerializeField] private float growthDuration = 10f; // 总生长时间
        [SerializeField] private GameObject harvestEffectPrefab; //动效的预制体

        public void SetCooldownTime(float time)
        {
            cooldownTime = time;
        }

        [Tooltip("收获资源类型")]
        [SerializeField] private HarvestType resourceType;

        [Tooltip("每次收获数量")]
        public int harvestAmount = 1;


        private SquareCell cell;
        private float currentCooldown; // 当前剩余冷却时间
        private bool canHarvest = false;// 是否可以收获

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
            harvestEffectPrefab = Resources.Load<GameObject>("UI/HarvestEffectPrefab");
            if (harvestEffectPrefab == null)
            {
                Debug.LogError("收获效果预制体未找到");
            }
        }
        /* private void Start()
        {
            if (resourceType == HarvestType.Branch)
            {
                isGrown = false;
                // 获取Cell子物体的SpriteRenderer
                var treeObj = transform.Find("CellTree");
                if (treeObj == null)
                {
                    Debug.LogError("找不到CellTree子物体");
                    return;
                }

                BranchRenderer = treeObj.GetComponent<SpriteRenderer>();
                if (BranchRenderer == null)
                {
                    Debug.LogError("CellTree上找不到SpriteRenderer组件");
                    return;
                }

                // 预加载两张生长阶段图片
                CellTreesprites = new Sprite[2];
                CellTreesprites[0] = Resources.Load<Sprite>("Images/Tree/huaShu2");
                CellTreesprites[1] = Resources.Load<Sprite>("Images/Tree/huaShu1");

                // 检查图片是否加载成功
                for (int i = 0; i < CellTreesprites.Length; i++)
                {
                    if (CellTreesprites[i] == null)
                    {
                        Debug.LogError($"无法加载图片: Images/Tree/huaShu{2 - i}");
                    }
                }

                if (BranchRenderer != null && CellTreesprites[0] != null)
                {
                    BranchRenderer.sprite = CellTreesprites[randomHuaShu];
                }
            }
        } */
        private void Start()
        {
            currentCooldown = cooldownTime;
        }

        public void TryHarvest()//尝试收获
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
            else if (canHarvest)//可收获时才执行收获操作
            {
                TryHarvest();
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
    
    private void TriggerHarvestEffect()
        {
            if (harvestEffectPrefab != null)
            {
                // 实例化动效预制体
                GameObject effect = Instantiate(harvestEffectPrefab, transform.position, Quaternion.identity, transform);
                TextMeshProUGUI effectText = effect.GetComponentInChildren<TextMeshProUGUI>();
                Image effectImage = effect.GetComponentInChildren<Image>();

                if (effectText != null) effectText.text = $"+{harvestAmount}";
                if (effectImage != null)
                    effectImage.sprite = Resources.Load<Sprite>($"UI/{resourceType}_Icon");

                // DOTween浮动动画
                effect.transform.DOMoveY(effect.transform.position.y + 1.5f, 0.8f).SetEase(Ease.OutQuad);
                effect.GetComponent<CanvasGroup>().DOFade(0, 0.8f).SetEase(Ease.OutQuad).OnComplete(() => Destroy(effect));
            }
        }
    }

}

