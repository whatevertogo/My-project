using TMPro;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using CDTU.Utils.TweenUtils;

public class CardUI : UIHoverClick, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    #region 组件引用
    [Header("卡片配置")]
    public TextMeshProUGUI CardText;
    [ReadOnly] public Image image;
    public CanvasGroup TextArea;
    public CanvasGroup cardCanvasGroup;
    public GameObject CardObject; 
    #endregion

    #region 动画配置
    [Header("动画配置")]
    public Vector2 moveOffset = new Vector2(0, 60);
    public float duration = 0.3f;
    [SerializeField] private int hoverSortingOrder = 10;
    public static int normalSortingOrder = 1;
    #endregion

    #region 私有字段
    private RectTransform rectTransform;
    private Canvas cardCanvas;
    private LayoutGroup layoutGroup;
    private Vector2 originalPosition;
    private Sequence currentSequence;
    private int originalSortingOrder;//记录原始的层级

    // 状态控制
    private bool isHovered = false;
    private bool isDragging = false;
    private bool isAnimating = false;

    // 防抖控制
    private float lastHoverTime = 0f;
    private const float hoverDebounceTime = 0.1f;

    // 数据
    private Card card;
    public static CardUI currentTopCard;
    #endregion

    #region 生命周期
    private void Awake()
    {
        InitializeComponents();
    }

    private void OnEnable()
    {
        InitializePosition();
    }

    private void Start()
    {
        InitializeUI();
    }

    private void OnDestroy()
    {
        CleanupAnimations();
    }
    #endregion

    #region 初始化方法
    private void InitializeComponents()
    {
        rectTransform = GetComponent<RectTransform>();
        cardCanvas = GetComponent<Canvas>();
        if (cardCanvas == null)
        {
            cardCanvas = gameObject.AddComponent<Canvas>();
        }
        cardCanvas.overrideSorting = true;

        // 获取父对象的 LayoutGroup
        layoutGroup = transform.parent?.GetComponent<LayoutGroup>();
    }    
    private void InitializePosition()
    {
        originalPosition = rectTransform.anchoredPosition;
        rectTransform.localScale = Vector3.one;
        SetSortingOrder(normalSortingOrder + 1);
        originalSortingOrder = normalSortingOrder + 1;
        SetSortingOrder(originalSortingOrder);
    }

    private void InitializeUI()
    {
        if (TextArea != null)
        {
            TextArea.alpha = 0f;
        }
    }
    #endregion

    #region 鼠标事件处理
    public override void OnPointerEnter(PointerEventData eventData)
    {
        // 如果正在拖拽，不处理悬停
        if (isDragging) return;

        // 更新状态
        isHovered = true;
        lastHoverTime = Time.time;

        if (currentTopCard == null)
        {
            currentTopCard = this;
        }

        // 如果当前正在执行动画，先停止
        if (isAnimating)
        {
            StopCurrentAnimation();
        }

        // 执行悬停动画
        AnimateToHovered();
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        // 如果正在拖拽，不处理退出
        if (isDragging) return;

        // 更新状态
        isHovered = false;
        lastHoverTime = Time.time;

        if (currentTopCard == this)
        {
            currentTopCard = null;
        }

        // 如果当前正在执行动画，先停止
        if (isAnimating)
        {
            StopCurrentAnimation();
        }

        // 执行恢复动画
        AnimateToNormal();
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        if (isDragging) return;
        DoClickBounce();
    }
    #endregion

    #region 拖拽事件处理
    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;
        isHovered = true;
        StopCurrentAnimation();

        // 禁用 LayoutGroup
        SetLayoutGroupEnabled(false);
        
        // 在拖拽时禁用射线阻挡
        if (cardCanvasGroup != null)
        {
            cardCanvasGroup.blocksRaycasts = false;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging) return;
        rectTransform.position = Input.mousePosition;
        transform.DOScale(0.3f, 0.1f).SetEase(Ease.OutBounce);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
        isHovered = false;

        // 恢复射线阻挡
        if (cardCanvasGroup != null)
        {
            cardCanvasGroup.blocksRaycasts = true;
        }

        SquareCell cell = InteractManager.Instance.GetCellUnderMouse();
        if (cell is not null && cell.IsExplored && cell.IsPlaceable)
        {
            HandleCardPlacement(cell);
        }
        else
        {
            ReturnToOriginalPosition();
        }
    }
    #endregion

    #region 动画方法
    private void AnimateTo(Vector2 targetPosition, float targetScale)
    {
        if (rectTransform is null) return;

        StopCurrentAnimation();
        SetLayoutGroupEnabled(false);
        isAnimating = true;

        // 创建新的动画序列
        currentSequence = DOTween.Sequence();

        // 位置和缩放动画
        currentSequence.Append(rectTransform.DOAnchorPos(targetPosition, duration).SetEase(Ease.InOutQuad));
        currentSequence.Join(transform.DOScale(targetScale, duration).SetEase(Ease.InOutQuad));

        // 动画完成回调
        currentSequence.OnComplete(() =>
        {
            isAnimating = false;
            SetLayoutGroupEnabled(true);
        });
    }

    private void AnimateToGenerate()
    {
        if (rectTransform == null || cardCanvasGroup == null) return;

        SetLayoutGroupEnabled(false); // 禁用布局

        rectTransform.localScale = Vector3.zero;
        cardCanvasGroup.alpha = 0f;

        Sequence spawnSequence = DOTween.Sequence();
        spawnSequence.Append(rectTransform.DOScale(1f, 0.3f).SetEase(Ease.OutBack));
        spawnSequence.Join(cardCanvasGroup.DOFade(1f, 0.3f));
        spawnSequence.OnComplete(() => SetLayoutGroupEnabled(true));// 动画完毕恢复布局
    }

    private void AnimateToHovered()
    {
        SetSortingOrder(hoverSortingOrder);
        AnimateTo(originalPosition + moveOffset, 1.1f);
        MyTweenUtils.FadeIn(TextArea, 0.2f);
        MyTweenUtils.FadeIn(cardCanvasGroup, 0.1f, 0.6f);
    }

    private void AnimateToNormal()
    {
        SetSortingOrder(originalSortingOrder);
        AnimateTo(originalPosition, 1.0f);
        MyTweenUtils.FadeOut(TextArea, 0.2f);
        MyTweenUtils.FadeOut(cardCanvasGroup, 0.1f, 1f);
    }

    private void DoClickBounce()
    {
        transform.DOKill();
        Sequence bounce = DOTween.Sequence();
        bounce.Append(transform.DOScale(1.15f, 0.1f));
        bounce.Append(transform.DOScale(1.0f, 0.15f).SetEase(Ease.OutBounce));
    }
    #endregion

    #region 辅助方法
    private bool CanProcessHoverEvent()
    {
        return !(Time.time - lastHoverTime < hoverDebounceTime || isDragging || isAnimating);
    }

    private void SetHoveredState(bool hoveredState)
    {
        isHovered = hoveredState;
        lastHoverTime = Time.time;

        if (hoveredState)
        {
            if (currentTopCard == null)
            {
                currentTopCard = this;
            }
        }
        else if (currentTopCard == this)
        {
            currentTopCard = null;
        }
    }

    private void SetSortingOrder(int order)
    {
        if (cardCanvas != null)
        {
            cardCanvas.sortingOrder = order;
        }
    }

    private void SetLayoutGroupEnabled(bool enabled)
    {
        if (layoutGroup != null)
        {
            layoutGroup.enabled = enabled;
        }
    }

    private void StopCurrentAnimation()
    {
        if (currentSequence != null)
        {
            currentSequence.Kill();
            currentSequence = null;
        }
        DOTween.Kill(transform);

        // 确保在停止动画时重新启用 LayoutGroup
        SetLayoutGroupEnabled(true);
    }

    private void CleanupAnimations()
    {
        StopCurrentAnimation();
        SetLayoutGroupEnabled(true);
    }

    private void HandleCardPlacement(SquareCell cell)
    {
        cell.UseCard(card);
        MyTweenUtils.FadeOut(cardCanvasGroup, 0.1f, 1f);
    }

    private void ReturnToOriginalPosition()
    {
        AnimateTo(originalPosition, 1.0f);
        MyTweenUtils.FadeOut(cardCanvasGroup, 0.1f, 1f);
    }
    #endregion

    #region 公共接口
    public void Bind(Card newCard)
    {
        card = newCard;
        UpdateUI();
        AnimateToGenerate();
    }

    public void UpdateUI()
    {
        if (card == null)
        {
            CardText.text = string.Empty;
            image.sprite = null;
        }
        else
        {
            CardText.text = card.CardName;
            image.sprite = card.CardSprite;
            //补充动效
        }
    }

    public void ClearUsedCard()
    {
        StopCurrentAnimation();
        CardManager.Instance?.RemoveCard(card);
        if (transform.parent != null)
        {
            Destroy(transform.parent.gameObject);
        }
        card = null;
    }
    #endregion
}
