
public class PlantedTreeBehavior : IGridTypeBehavior
{
    public void ApplyBehavior(SquareCell cell)
    {
        cell.IsPlaceable = false;
        GridManager.Instance.AllDontMoveCells.Add(cell);
    }
    
    public void OnInteract(SquareCell cell)
    {
        // 这里可以添加特定于树木格子的交互逻辑
        // 例如：摇晃树木、掉落水果等
    }
}