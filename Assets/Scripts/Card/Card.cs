using TMPro;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

public class Card : UIHoverClick
{
    [Header("卡片配置")]
    public TextMeshProUGUI CardText;
    public Image image;
    public Vector2 moveOffset = new Vector2(0, 60); // 上移偏移量
    public float duration = 0.3f;                   // 动画时间

    [Header("层级设置")]
    [SerializeField] private int hoverSortingOrder = 10;  // 悬停时的排序顺序
    [SerializeField] private int normalSortingOrder = 1;  // 正常时的排序顺序

    private Sequence currentSequence;
    private bool isHovered = false;
    private Vector2 originalPosition;
    private RectTransform rectTransform;
    private Canvas cardCanvas;

    // 跟踪当前置顶的卡片
    public static Card currentTopCard;

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
    }

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
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log($"Card '{CardText.text}' clicked!");
        DoClickBounce();
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

    public void UpdateCardText(string newText)
    {
        CardText.text = newText;
    }
}
