using UnityEngine;

public class DefaultHoverHandler : ICellHoverHandler
{
    public void OnHoverEnter(SquareCell cell)
    {
        // 默认悬停逻辑
        Debug.Log($"悬停在格子 {cell.Coordinates} 上！");
    }

    public void OnHoverExit(SquareCell cell)
    {
        // 默认悬停结束逻辑
        Debug.Log($"离开格子 {cell.Coordinates}！");
    }
}
