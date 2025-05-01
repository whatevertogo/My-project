using System.Collections.Generic;
using UnityEngine;

public class SquareMetrics
{
    public static float cellSize = 1f; //正方形的边长

    // SideLength 的倒数，自动推导，统一管理
    public static float InverseSideLength => 1f / cellSize;

    /// <summary>
    /// 一个Vector3数组，表示以中心为原点的正方形4个顶点坐标
    /// </summary>
    public static readonly Vector3[] corners = {
        new Vector3(-cellSize/2f, -cellSize/2f, 0f), // 左下
        new Vector3(cellSize/2f, -cellSize/2f, 0f),  // 右下
        new Vector3(cellSize/2f, cellSize/2f, 0f),   // 右上
        new Vector3(-cellSize/2f, cellSize/2f, 0f),  // 左上
        new Vector3(-cellSize/2f, -cellSize/2f, 0f), // 闭合回左下
    };

    // 边的前一个顶点
    public static Vector3 GetFirstCorner(SquareDirection direction)
    {
        return corners[(int)direction];
    }
    

    // 边的后一个顶点
    public static Vector3 GetSecondCorner(SquareDirection direction)
    {
        return corners[(int)direction + 1];
    }

}
