using System;
using UnityEngine;

[CreateAssetMenu(fileName = "CardData", menuName = "ScriptableObjects/CardData", order = 1)]
[Serializable]
public class CardData : ScriptableObject
{
    public int id;
    public string cardName;
    public CardType cardType;
    public Sprite cardSprite;

    private CardEffect effect;

    [TextArea(3, 10)]
    public string description;
    public CardData(int id, string name, string desc, CardType type, Sprite sprite = default)
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