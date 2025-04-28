
using UnityEngine;

public class TreeHoverHandler :ICellHoverHandler
{
    public void OnHoverEnter(SquareCell cell)
    {
        //TODO-树地块的悬停效果
        Debug.Log("悬停树");
    }

    public void OnHoverExit(SquareCell cell)
    {
        Debug.Log("悬停出树");
    }

}