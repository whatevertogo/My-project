using HexGame.Harvest;

public class SimpleSquareBehavior : IGridTypeBehavior
{
    public void ApplyBehavior(SquareCell cell)
    {
        cell.IsPlaceable = true;
    }
    
    public void OnInteract(SquareCell cell)
    {
        // 简单方格的交互逻辑，移除对不存在方法的调用
        // 如果需要自定义交互逻辑，可以在这里添加
    }
}