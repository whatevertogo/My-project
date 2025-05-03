using UnityEngine;
using HexGame.Harvest;

public class PlantTreeEffect : CardEffect
{
    public PlantTreeEffect(Card card) : base(card) { }

    public override bool CanUse(SquareCell targetCell)
    {
        // 只能在空白格子上种树
        return targetCell.GetGridType() == GridType.SimpleSquare;
    }
    public override void Execute(SquareCell targetCell)
    {
        if (!CanUse(targetCell)) return;

        // 将格子转为树格子
        targetCell.SetGridType(GridType.PlantedTree);

        var cellPineCone = new GameObject("CellTree");

        cellPineCone.transform.SetParent(targetCell.transform);
        var spriteRenderer = cellPineCone.AddComponent<SpriteRenderer>();

        cellPineCone.transform.localPosition = new Vector3(0, 0.5f, 0);

        spriteRenderer.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);


        // 添加可收获组件
        Harvestable harvestable = targetCell.gameObject.AddComponent<Harvestable>();
        harvestable.SetResourceType(HarvestType.Branch);
        harvestable.SetHarvestAmount(1);

        Debug.Log($"在 {targetCell.Coordinates} 种植了树");

        // 直接移除当前执行的卡牌
        CardManager.Instance.RemoveCard(card);
    }
}