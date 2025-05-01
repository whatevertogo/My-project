using UnityEngine;

public class Card
{
    public CardData CardData { get; private set; }
    private CardEffect effect;

    public Card(CardData cardData)
    {
        CardData = cardData;

        // 动态创建对应的效果
        effect = CardEffectFactory.CreateEffect(this);
    }

    public string CardName => CardData.cardName;
    public string Description => CardData.description;
    public CardType CardType => CardData.cardType;
    public Sprite CardSprite => CardData.cardSprite;

    // 检查卡牌是否可以在目标格子上使用
    public bool CanUseOn(SquareCell targetCell)
    {
        if (effect is null) return false;
        return effect.CanUse(targetCell);
    }

    // 在目标格子上使用卡牌
    public void UseOn(SquareCell targetCell)
    {
        if (effect is null || !CanUseOn(targetCell)) return;
        effect.Execute(targetCell);
    }
}