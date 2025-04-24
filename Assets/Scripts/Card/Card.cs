using TMPro;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

public class Card : UIHoverClick
{
    public TextMeshProUGUI CardText;
    public Image image;
    public Vector2 moveOffset = new Vector2(0, 60); // 上移偏移量
    public float duration = 0.3f;                   // 动画时间

    private Sequence currentSequence;
    private bool isHovered = false;
    private Vector2 originalPosition;
    private RectTransform rectTransform;

    public static Card currentTopCard;


    private void OnEnable()
    {
        rectTransform = GetComponent<RectTransform>();
        originalPosition = rectTransform.anchoredPosition;
        rectTransform.localScale = Vector3.one;
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        if (isHovered) return;
        isHovered = true;
        if (currentTopCard is null)
        {
            currentTopCard = this;
        }
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
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Card clicked!");
        DoClickBounce();
    }

    private void AnimateTo(Vector2 targetPosition, float targetScale)
    {
        currentSequence?.Kill();
        currentSequence = DOTween.Sequence();
        currentSequence.Append(rectTransform.DOAnchorPos(targetPosition, duration).SetEase(Ease.InOutQuad));
        currentSequence.Join(transform.DOScale(targetScale, duration).SetEase(Ease.InOutQuad));
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
