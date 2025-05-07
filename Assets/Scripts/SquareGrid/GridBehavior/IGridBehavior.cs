public interface IGridTypeBehavior
{
    void ApplyBehavior(SquareCell cell);
    
    /// <summary>
    /// 处理与格子的交互逻辑
    /// </summary>
    /// <param name="cell">要交互的格子</param>
    void OnInteract(SquareCell cell);
}

