using UnityEngine;
using HexGame.Harvest;

public class PlantPineConeEffect : CardEffect
{
    public PlantPineConeEffect(Card card) : base(card)
    {
    }

    public override bool CanUse(SquareCell targetCell)
    {
        // 只能在空白格子上种稻谷
        return targetCell.GetGridType() == GridType.SimpleSquare;
    }

    public override void Execute(SquareCell targetCell)
    {
        if (!CanUse(targetCell)) return;

        // 将格子转为松树格子
        targetCell.SetGridType(GridType.PlantedTree);
        var cellPineCone = new GameObject("CellPineCone");

        cellPineCone.transform.SetParent(targetCell.transform);
        var spriteRenderer = cellPineCone.AddComponent<SpriteRenderer>();

        cellPineCone.transform.localPosition = new Vector3(0, 0.5f, 0);

        spriteRenderer.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);

        // spriteRenderer.transform.rotation = Quaternion.Euler(-10, 0, 0); // 设置旋转
        spriteRenderer.sortingLayerName = "Behavior";
        spriteRenderer.sortingOrder = 3;// 设置渲染顺序

        // 添加可收获组件
        Harvestable harvestable = targetCell.gameObject.AddComponent<Harvestable>();
        harvestable.SetResourceType(HarvestType.PineCone);
        harvestable.SetHarvestAmount(1);

        Debug.Log($"在 {targetCell.Coordinates} 种植了松树");

        // 直接移除当前执行的卡牌
        CardManager.Instance.RemoveCard(card);
    }
}