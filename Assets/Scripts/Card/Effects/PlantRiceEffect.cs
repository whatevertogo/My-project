using UnityEngine;

public class PlantRiceEffect : CardEffect
{
    public PlantRiceEffect(Card card) : base(card) { }

    public override bool CanUse(SquareCell targetCell)
    {
        // 只能在空白格子上种稻谷
        return targetCell.GetGridType() == GridType.SimpleSquare;
    }

    public override void Execute(SquareCell targetCell)
    {
        if (!CanUse(targetCell)) return;

        // 将格子转为稻田格子
        targetCell.SetGridType(GridType.PlantedRice);

        

        // TODO: 添加稻谷生长组件或者效果

        Debug.Log($"在 {targetCell.Coordinates} 种植了稻谷");

        // 直接移除当前执行的卡牌
        CardManager.Instance.RemoveCard(card);
    }
}