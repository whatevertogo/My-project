


using UnityEngine;

public class FeatherHoverHandler : ICellHoverHandler
{
    private readonly DefaultHoverHandler defaultHandler = new DefaultHoverHandler();
    public void OnHoverEnter(SquareCell cell)
    {
        //TODO-羽毛地块的悬停效果
        Debug.Log("悬停羽毛");
        defaultHandler.OnHoverEnter(cell); // 调用默认逻辑
    }

    public void OnHoverExit(SquareCell cell)
    {
        Debug.Log("悬停出羽毛");
        defaultHandler.OnHoverExit(cell);// 调用默认逻辑
    }
}