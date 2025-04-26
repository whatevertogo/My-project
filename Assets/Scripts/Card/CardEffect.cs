using UnityEngine;

public abstract class CardEffect
{
    protected CardData card;

    public CardEffect(CardData card)
    {
        this.card = card;
    }

    // 检查是否可以使用
    public virtual bool CanUse(SquareCell targetCell)
    {
        // 基本检查，如目标格子是否存在等
        return targetCell != null;
    }

    // 使用卡牌效果
    public abstract void Execute(SquareCell targetCell);
}