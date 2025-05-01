using UnityEngine;

public static class CardEffectFactory
{
    public static CardEffect CreateEffect(Card card)
    {
        switch (card.CardType)
        {
            case CardType.tree:
                return new PlantTreeEffect(card);
            case CardType.Paddy:
                return new PlantRiceEffect(card);
            case CardType.Feather:
                return new PlantFeatherEffect(card);
            default:
                Debug.LogWarning($"未实现的卡牌类型: {card.CardType}");
                return null;
        }
    }
}