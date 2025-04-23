using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 表示网格中的一个方形单元格
/// </summary>
public interface ISquareCell
{
    void Init(SquareCoordinates coordinates);
    SquareCoordinates GetCoordinates();
    Vector3 GetWorldPosition();
    void SetNeighbor(SquareDirection direction, SquareCell neighbor);
    SquareCell GetNeighbor(SquareDirection direction);
    IReadOnlyList<SquareCell> GetneighborsAndSelf();
    bool TryGetNeighbor(SquareDirection direction, out SquareCell neighbor);

}