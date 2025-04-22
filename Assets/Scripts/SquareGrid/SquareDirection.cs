public enum SquareDirection
{
    N,
    S,
    W,
    E,
    NW,
    NE,
    SW,
    SE,
    Center
}

public static class SquareDirectionExtensions
{
    /// <summary>
    /// 获取相反方向
    /// </summary>
    public static SquareDirection Opposite(this SquareDirection direction)
    {
        switch (direction)
        {
            case SquareDirection.N: return SquareDirection.S;
            case SquareDirection.S: return SquareDirection.N;
            case SquareDirection.W: return SquareDirection.E;
            case SquareDirection.E: return SquareDirection.W;
            case SquareDirection.NW: return SquareDirection.SE;
            case SquareDirection.SE: return SquareDirection.NW;
            case SquareDirection.NE: return SquareDirection.SW;
            case SquareDirection.SW: return SquareDirection.NE;
            case SquareDirection.Center: return SquareDirection.Center;
            default:
                throw new System.ArgumentException("没有此方向", nameof(direction));
        }
    }

    /// <summary>
    /// 获取方向对应的坐标偏移
    /// </summary>
    public static (int dx, int dy) GetOffset(this SquareDirection direction)
    {
        switch (direction)
        {
            case SquareDirection.N: return (0, 1);
            case SquareDirection.S: return (0, -1);
            case SquareDirection.E: return (1, 0);
            case SquareDirection.W: return (-1, 0);
            case SquareDirection.NW: return (-1, 1);
            case SquareDirection.NE: return (1, 1);
            case SquareDirection.SW: return (-1, -1);
            case SquareDirection.SE: return (1, -1);
            case SquareDirection.Center: return (0, 0);
            default:
                throw new System.ArgumentException("没有此方向", nameof(direction));
        }
    }
}