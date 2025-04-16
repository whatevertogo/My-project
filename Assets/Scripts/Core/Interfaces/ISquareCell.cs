using UnityEngine;

/// <summary>
/// 表示网格中的一个方形单元格
/// </summary>
public interface ISquareCell
{
    SquareCoordinates GetCoordinates();
    Vector3 GetWorldPosition();
}