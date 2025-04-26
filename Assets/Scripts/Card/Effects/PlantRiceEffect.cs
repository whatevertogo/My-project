using UnityEngine;

public class PlantRiceEffect : CardEffect
{
    public PlantRiceEffect(CardData card) : base(card) { }
    
    public override bool CanUse(SquareCell targetCell)
    {
        if (!base.CanUse(targetCell)) return false;
        
        // 只能在空白格子上种稻谷
        return targetCell.GetGridType() == GridType.SimpleSquare;
    }
    
    public override void Execute(SquareCell targetCell)
    {
        if (!CanUse(targetCell)) return;
        
        // 将格子转为稻田格子
        targetCell.SetGridType(GridType.PlantedRice);
        
        // todo-添加稻谷生长组件或者效果
        
        Debug.Log($"在 {targetCell.Coordinates} 种植了稻谷");
        
        // 使用后移除卡牌
        CardManager.Instance.RemoveCard(card);
    }
}