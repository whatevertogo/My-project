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

        var CellTree = new GameObject("CellTree");
        CellTree.transform.position = targetCell.transform.position;

        // 将树对象设置为格子对象的子对象
        CellTree.transform.SetParent(targetCell.transform);
        var spriteRenderer = CellTree.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = Resources.Load<Sprite>("Images/Tree/songShu");

        // 添加可收获组件
        Harvestable harvestable = targetCell.gameObject.AddComponent<Harvestable>();
        harvestable.SetResourceType(HarvestType.Branch);
        harvestable.SetHarvestAmount(1);

        Debug.Log($"在 {targetCell.Coordinates} 种植了树");

        // 直接移除当前执行的卡牌
        CardManager.Instance.RemoveCard(card);
    }
}