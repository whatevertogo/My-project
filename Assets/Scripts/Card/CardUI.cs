using TMPro;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using CDTU.Utils.TweenUtils;

public class CardUI : UIHoverClick
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

    private Sequence currentSequence;
    private bool isHovered = false;
    private Vector2 originalPosition;
    private RectTransform rectTransform;
    public Canvas cardCanvas;
    #endregion
    private Card card;
    public static CardUI currentTopCard;

    private void OnEnable()
    {
        rectTransform = GetComponent<RectTransform>();
        originalPosition = rectTransform.anchoredPosition;
        rectTransform.localScale = Vector3.one;

        // 获取或添加 Canvas 组件
        cardCanvas = GetComponent<Canvas>();
        if (cardCanvas == null)
        {
            cardCanvas = gameObject.AddComponent<Canvas>();
            cardCanvas.overrideSorting = true;

            // 添加 GraphicRaycaster 以确保仍能接收点击事件
            if (GetComponent<GraphicRaycaster>() == null)
            {
                gameObject.AddComponent<GraphicRaycaster>();
            }
        }


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
        TextArea.GetComponent<CanvasGroup>().alpha = 0f;
    }

    #region 鼠标动画交互效果
    public override void OnPointerEnter(PointerEventData eventData)
    {
        if (isHovered) return;
        isHovered = true;

        if (currentTopCard == null)
        {
            currentTopCard = this;
        }

        // 将卡片置于顶层
        SetSortingOrder(hoverSortingOrder);

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
        if (cardCanvas != null)
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
        if (card == null)
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
}
