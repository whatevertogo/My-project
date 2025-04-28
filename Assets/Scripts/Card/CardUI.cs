using TMPro;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using CDTU.Utils.TweenUtils;

public class CardUI : UIHoverClick, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    #region 卡牌ui
    [Header("卡片配置")]
    public TextMeshProUGUI CardText;
    [ReadOnly]
    public Image image;
    public CanvasGroup TextArea;
    public Vector2 moveOffset = new Vector2(0, 60); // 上移偏移量
    public float duration = 0.3f;                   // 动画时间
    #endregion
    #region 动效设置
    [Header("层级设置")]
    [SerializeField] private int hoverSortingOrder = 10;  // 悬停时的排序顺序
    public static int normalSortingOrder = 1;  // 正常时的排序顺序
    [Header("动画设置")]
    private Sequence currentSequence;
    private bool isHovered = false;
    private Vector2 originalPosition;
    private RectTransform rectTransform;
    [ReadOnly]
    public Canvas cardCanvas;
    public CanvasGroup cardCanvasGroup;


    #endregion

    private Card card;
    public static CardUI currentTopCard;


    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        //todo-改天简化这个逻辑
        if (gameObject.TryGetComponent<Canvas>(out cardCanvas))
        {
            cardCanvas.overrideSorting = true;
        }
        else
        {
            cardCanvas = gameObject.AddComponent<Canvas>();
            cardCanvas.overrideSorting = true;
        }
        
    }

    private void OnEnable()
    {
        originalPosition = rectTransform.anchoredPosition;
        rectTransform.localScale = Vector3.one;

        // 设置初始排序顺序
        cardCanvas.sortingOrder = normalSortingOrder;
        normalSortingOrder++;
    }
    private void Start()
    {
        if (TextArea is null)
        {
            Debug.LogError("TextArea is null, please assign it in the inspector.");
            return;
        }
        TextArea.alpha = 0f;
    }

    #region 鼠标动画交互效果
    public override void OnPointerEnter(PointerEventData eventData)
    {
        if (isHovered) return;
        isHovered = true;

        if (currentTopCard is null)
        {
            currentTopCard = this;
        }

        // 将卡片置于顶层
        SetSortingOrder(hoverSortingOrder);

        MyTweenUtils.FadeIn(cardCanvasGroup, 0.1f, 0.6f);

        // 动画改变位置和缩放
        AnimateTo(originalPosition + moveOffset, 1.1f);
        //显示文本
        MyTweenUtils.FadeIn(TextArea, 0.2f);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        if (!isHovered) return;
        isHovered = false;

        if (currentTopCard == this)
        {
            currentTopCard = null;
        }

        // 恢复正常层级
        SetSortingOrder(normalSortingOrder);


        // 添加回下降动画，让卡片回到原位
        AnimateTo(originalPosition, 1.0f);
        //隐藏文本
        MyTweenUtils.FadeOut(TextArea, 0.2f);
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log($"Card '{CardText.text}' clicked!");
        DoClickBounce();
        //todo- 这里可以添加点击卡牌的逻辑，比如使用卡牌效果
        // ClickManager.Instance.SetSelectedCard(cardData);
        transform.DOScale(1.0f, 0.1f);
    }

    private void AnimateTo(Vector2 targetPosition, float targetScale)
    {
        // 停止当前动画序列
        currentSequence?.Kill();

        // 创建新的动画序列
        currentSequence = DOTween.Sequence();
        currentSequence.Append(rectTransform.DOAnchorPos(targetPosition, duration).SetEase(Ease.InOutQuad));
        currentSequence.Join(transform.DOScale(targetScale, duration).SetEase(Ease.InOutQuad));
    }

    private void SetSortingOrder(int order)
    {
        if (cardCanvas is not null)
        {
            cardCanvas.sortingOrder = order;
        }
    }

    public void DoClickBounce()
    {
        transform.DOKill();
        Sequence bounce = DOTween.Sequence();
        bounce.Append(transform.DOScale(1.15f, 0.1f));
        bounce.Append(transform.DOScale(1.0f, 0.15f).SetEase(Ease.OutBounce));
    }
    #endregion

    public void Bind(Card newCard)
    {
        card = newCard;

        // 更新 UI 显示
        UpdateUI();
    }

    // 更新 UI 显示
    public void UpdateUI()
    {
        if (card is null)
        {
            CardText.text = string.Empty;
            image.sprite = null;
        }
        else
        {
            CardText.text = card.CardName;
            image.sprite = card.CardSprite;
        }
    }
    private bool isDragging = false;

    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;
        isHovered = true;
        currentSequence?.Kill(); // 停止任何正在进行的动画
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging) return;
        // 直接设置位置而不是使用动画
        Vector2 mousePos = Input.mousePosition;
        rectTransform.position = mousePos;
        Debug.Log("Dragging card to position: " + mousePos+InteractManager.Instance.GetCellUnderMouse());
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
        isHovered = false;

        SquareCell cell = InteractManager.Instance.GetCellUnderMouse();
        if (cell is null)
        {
            // 只有在没有找到目标格子时和没有拖拽物体的时候才返回原位
            if (eventData.pointerDrag is null) return;
            AnimateTo(originalPosition, 1.0f);
        }
        else
        {
            // TODO: 实现卡牌放置逻辑
            Debug.Log($"Card placed on cell: {cell.name}");
        }
    }
}
