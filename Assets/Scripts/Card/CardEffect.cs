using UnityEngine;

public abstract class CardEffect
{
    protected Card card; // 当前执行的卡牌

    public CardEffect(Card card)
    {
        this.card = card;
    }

    // 检查是否可以使用
    public abstract bool CanUse(SquareCell targetCell);

    // 执行卡牌效果
    public abstract void Execute(SquareCell targetCell);
}