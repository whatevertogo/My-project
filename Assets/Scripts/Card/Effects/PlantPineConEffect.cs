using UnityEngine;
using HexGame.Harvest; // 添加命名空间引用

public class PlantPineConeEffect : CardEffect
{
    public PlantPineConeEffect(Card card) : base(card) { }

    public override bool CanUse(SquareCell targetCell)
    {
        // 只能在空白格子上种稻谷
        return targetCell.GetGridType() == GridType.SimpleSquare;
    }
    public override void Execute(SquareCell targetCell)
    {
        if (!CanUse(targetCell)) return;

        // 将格子转为稻田格子
        targetCell.SetGridType(GridType.PlantedTree);

        GridManager.Instance.AllDontMoveCells.Add(targetCell); // 将格子添加到不可移动列表中

        // 添加可收获组件
        var harvestable = targetCell.gameObject.AddComponent<Harvestable>();
        harvestable.SetResourceType(HarvestType.PineCone);
        harvestable.SetHarvestAmount(1);

        Debug.Log($"在 {targetCell.Coordinates} 种植了羽毛");

        // 直接移除当前执行的卡牌
        CardManager.Instance.RemoveCard(card);
    }
}