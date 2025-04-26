using UnityEngine;
using System.Linq; // 添加 LINQ 命名空间

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

        // TODO: 添加树生长组件或者效果

        Debug.Log($"在 {targetCell.Coordinates} 种植了一棵树");

        // 直接移除当前执行的卡牌
        CardManager.Instance.RemoveCard(card);
    }
}