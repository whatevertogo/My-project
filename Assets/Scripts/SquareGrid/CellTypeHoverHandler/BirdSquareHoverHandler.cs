using UnityEngine;

public class BirdSquareHoverHandler : ICellHoverHandler
{
    private readonly DefaultHoverHandler defaultHandler = new DefaultHoverHandler();

    public void OnHoverEnter(SquareCell cell)
    {
        Debug.Log($"鸟格子 {cell.Coordinates} 悬停，显示小鸟特效！");
        // 自定义逻辑
        defaultHandler.OnHoverEnter(cell); // 调用默认逻辑
    }

    public void OnHoverExit(SquareCell cell)
    {
        Debug.Log($"鸟格子 {cell.Coordinates} 悬停结束！");
        defaultHandler.OnHoverExit(cell); // 调用默认逻辑
    }
}
