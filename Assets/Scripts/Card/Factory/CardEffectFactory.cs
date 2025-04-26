using UnityEngine;

public static class CardEffectFactory
{
    public static CardEffect CreateEffect(CardData card)
    {
        switch (card.cardType)
        {
            case CardType.tree:
                return new PlantTreeEffect(card);
            
            case CardType.Paddy:
                return new PlantRiceEffect(card);
                
            // 可以轻松添加更多卡牌类型
            
            default:
                Debug.LogWarning($"未实现的卡牌类型: {card.cardType}");
                return null;
        }
    }
}