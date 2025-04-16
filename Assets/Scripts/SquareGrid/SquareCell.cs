using UnityEngine;
using System.Collections.Generic;

public class SquareCell : ISquareCell
{
    public SquareCoordinates Coordinates { get; private set; }

    // 存储邻居的列表
    private readonly SquareCell[] neighbors = new SquareCell[4];

    // 提供设置邻居的方法（应由网格管理器调用）
    public void SetNeighbor(SquareDirection direction, SquareCell neighbor)
    {
        int index = (int)direction;
        if (index >= 0 && index < neighbors.Length)
            neighbors[index] = neighbor;
    }

    public SquareCell(SquareCoordinates coordinates)
    {
        Coordinates = coordinates;
    }



    public SquareCell(int x, int y) : this(new SquareCoordinates(x, y)) { }

    // 通过 Square.SideLength 的倒数进行世界坐标转格子坐标
    public SquareCell(Vector3 worldPosition) : this(
        new SquareCoordinates(
            Mathf.RoundToInt(worldPosition.x * Square.InverseSideLength),
            Mathf.RoundToInt(worldPosition.y * Square.InverseSideLength)))
    { }

    /// <summary>
    /// 获取指定方向的邻居
    /// </summary>
    public SquareCell GetNeighbor(SquareDirection direction)
    {
        int index = (int)direction;
        if (index >= 0 && index < neighbors.Length)
            return neighbors[index];
        return null;
    }

    /// <summary>
    /// 获取所有邻居（只读）
    /// </summary>
    public IReadOnlyList<SquareCell> GetNeighbors()
    {
        return System.Array.AsReadOnly(neighbors);
    }

    /// <summary>
    /// 获取指定方向的邻居，如果不存在则返回false
    /// </summary>
    public bool TryGetNeighbor(SquareDirection direction, out SquareCell neighbor)
    {
        int index = (int)direction;
        if (index >= 0 && index < neighbors.Length && neighbors[index] != null)
        {
            neighbor = neighbors[index];
            return true;
        }
        neighbor = null;
        return false;
    }

    public Vector3 GetWorldPosition()
    {
        return Coordinates.ToWorldPosition();
    }

    #region 运算符重载
    public override bool Equals(object obj)
    {
        if (obj is SquareCell other)
        {
            return Coordinates.Equals(other.Coordinates);
        }
        return false;
    }

    public override int GetHashCode()
    {
        return Coordinates.GetHashCode();
    }

    public override string ToString()
    {
        return $"SquareCell({Coordinates})";
    }

    public SquareCoordinates GetCoordinates()
    {
        throw new System.NotImplementedException();
    }
    #endregion
}
