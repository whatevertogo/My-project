using System;
using UnityEngine;

[Serializable]
public class CardData
{
    public int id;
    public string cardName;
    public string description;
    public CardType cardType;
    public Sprite cardSprite;

    private CardEffect cardEffect;

    public CardData(int id, string name, string desc, CardType type, Sprite sprite)
    {
        this.id = id;
        this.cardName = name;
        this.description = desc;
        this.cardType = type;
        this.cardSprite = sprite;
    }

    // 检查卡牌是否可以在目标格子上使用
    public bool CanUseOn(SquareCell targetCell)
    {
        if (effect == null) return false;
        return effect.CanUse(targetCell);
    }
    
    // 在目标格子上使用卡牌
    public void UseOn(SquareCell targetCell)
    {
        if (effect == null || !CanUseOn(targetCell)) return;
        effect.Execute(targetCell);
    }
}