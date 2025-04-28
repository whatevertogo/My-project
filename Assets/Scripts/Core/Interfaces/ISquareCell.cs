using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 表示网格中的一个方形单元格
/// </summary>
public interface ISquareCell
{    /// <summary>
    /// 初始化格子的坐标
    /// </summary>
    /// <param name="coordinates">格子的坐标</param>
    void Init(SquareCoordinates coordinates);

    /// <summary>
    /// 获取格子的坐标
    /// </summary>
    /// <returns>格子的坐标</returns>
    SquareCoordinates GetCoordinates();

    /// <summary>
    /// 获取格子的世界坐标
    /// </summary>
    /// <returns>格子的世界坐标</returns>
    Vector3 GetWorldPosition();

    /// <summary>
    /// 设置指定方向的邻居格子
    /// </summary>
    /// <param name="direction">邻居方向</param>
    /// <param name="neighbor">邻居格子</param>
    void SetNeighbor(SquareDirection direction, SquareCell neighbor);

    /// <summary>
    /// 获取指定方向的邻居格子
    /// </summary>
    /// <param name="direction">邻居方向</param>
    /// <returns>邻居格子</returns>
    SquareCell GetNeighbor(SquareDirection direction);

    /// <summary>
    /// 获取所有有效的邻居（包含自身）
    /// </summary>
    /// <returns>所有有效邻居的列表</returns>
    List<SquareCell> GetNeighborsAndSelf();

    /// <summary>
    /// 尝试获取指定方向的邻居格子
    /// </summary>
    /// <param name="direction">邻居方向</param>
    /// <param name="neighbor">输出的邻居格子</param>
    /// <returns>是否获取成功</returns>
    bool TryGetNeighbor(SquareDirection direction, out SquareCell neighbor);

}