using UnityEngine;

public class PlantFeatherEffect : CardEffect
{
    public PlantFeatherEffect(Card card) : base(card) { }

    public override bool CanUse(SquareCell targetCell)
    {
        // 只能在空白格子上种稻谷
        return targetCell.GetGridType() == GridType.SimpleSquare;
    }

    public override void Execute(SquareCell targetCell)
    {
        if (!CanUse(targetCell)) return;

        // 将格子转为稻田格子
        targetCell.SetGridType(GridType.PlantedFeather);

        GridManager.Instance.AllDontMoveCells.Add(targetCell); // 将格子添加到不可移动列表中


        // TODO: 添加羽毛效果

        Debug.Log($"在 {targetCell.Coordinates} 种植了稻谷");

        // 直接移除当前执行的卡牌
        CardManager.Instance.RemoveCard(card);
    }
}