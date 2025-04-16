using System;
using System.ComponentModel;
using UnityEngine;

[Serializable]
public struct SquareCoordinates : IEquatable<SquareCoordinates>
{
    [SerializeField] private int x, y;

    [ReadOnly(true)]
    public int X => x;
    [ReadOnly(true)]
    public int Y => y;

    public SquareCoordinates(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    // 坐标运算
    public static SquareCoordinates operator +(SquareCoordinates a, SquareCoordinates b)
        => new SquareCoordinates(a.x + b.x, a.y + b.y);

    // 计算距离
    public int DistanceTo(SquareCoordinates other)
        => Math.Abs(x - other.x) + Math.Abs(y - other.y);

    // 判断相邻
    public bool IsAdjacentTo(SquareCoordinates other)
        => DistanceTo(other) == 1;

    // 转换为世界坐标
    public Vector3 ToWorldPosition()
        => new Vector3(x * Square.SideLength, y * Square.SideLength, 0);

    public bool Equals(SquareCoordinates other)
        => x == other.x && y == other.y;

    public override bool Equals(object obj)
        => obj is SquareCoordinates other && Equals(other);

    public override int GetHashCode()
        => HashCode.Combine(x, y);
}

