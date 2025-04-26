using UnityEngine;

public class PlantTreeEffect : CardEffect
{
    public PlantTreeEffect(CardData card) : base(card) { }
    
    public override bool CanUse(SquareCell targetCell)
    {
        if (!base.CanUse(targetCell)) return false;
        
        // 只能在空白格子上种树
        return targetCell.GetGridType() == GridType.SimpleSquare;
    }
    
    public override void Execute(SquareCell targetCell)
    {
        if (!CanUse(targetCell)) return;
        
        // 将格子转为树格子
        targetCell.SetGridType(GridType.PlantedTree);
        
        // 添加树生长组件
        PlantGrowth growth = targetCell.gameObject.AddComponent<PlantGrowth>();
        growth.Initialize(card, HarvestType.Branch, 10f, 30f);
        
        Debug.Log($"在 {targetCell.Coordinates} 种植了一棵树");
        
        // 使用后移除卡牌
        CardManager.Instance.RemoveCard(card);
    }
}