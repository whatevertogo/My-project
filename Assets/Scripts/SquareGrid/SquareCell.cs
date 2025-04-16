using UnityEngine;
using System.Collections.Generic;
using System.ComponentModel;

public class SquareCell : MonoBehaviour, ISquareCell
{
    public SquareCoordinates Coordinates { get; set; }

    // 存储邻居的列表
    [ReadOnly(true)]
    [SerializeField] private readonly SquareCell[] neighbors = new SquareCell[4];

    public Vector3 GetWorldPosition()
    {
        return Coordinates.ToWorldPosition();
    }

    public void Init(SquareCoordinates coordinates)
    {
        this.Coordinates = coordinates;
    }

    #region 邻居方法
    // 提供设置邻居的方法（应由网格管理器调用）
    public void SetNeighbor(SquareDirection direction, SquareCell neighbor)
    {
        int index = (int)direction;
        if (index >= 0 && index < neighbors.Length)
            neighbors[index] = neighbor;
    }
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
    #endregion

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
