using System.Collections.Generic;
using System.Linq;
using CDTU.Utils;
using UnityEngine;

public class GridManager : Singleton<GridManager>
{
    [Header("网格大小配置")]
    public int width = 10;
    public int height = 10;

    public Vector3 GetGridCenter() => new Vector3(width / 2, height / 2, 0);

    [Header("网格类型配置和需要的BridSquare数量")]
    public GridTypeConfig gridTypeConfig; // 网格类型配置文件
    public int requiredCount = 2; // 至少需要的 BirdSquare 数量
    private SquareCell[,] cells;
    public IEnumerable<SquareCell> AllCells
    {
        get
        {
            foreach (var cell in cells)
                yield return cell;
        }
    }
    public HashSet<SquareCell> AllBridCells = new();

    protected override void Awake()
    {
        // 调用基类的 Awake 方法
        base.Awake();
        // 初始化 RandomGridType
        RandomGridType.Initialize(gridTypeConfig);
        //生成网格
        GenerateGrid();
    }

    /// <summary>
    /// 生成网格并设置每个格子的邻居关系。
    /// </summary>
    public void GenerateGrid()
    {
        // 初始化二维数组，用于存储网格中的所有格子
        cells = new SquareCell[width, height];
        // 遍历网格的每一列
        for (int x = 0; x < width; x++)
        {
            // 遍历网格的每一行
            for (int y = 0; y < height; y++)
            {
                // 创建一个四边形游戏对象作为格子
                GameObject cellObj = GameObject.CreatePrimitive(PrimitiveType.Quad);
                // 为格子对象命名，包含其坐标信息
                cellObj.name = $"Cell_{x}_{y}";
                // 设置格子对象的位置，根据其坐标和格子大小计算
                cellObj.transform.position = new Vector3(x * SquareMetrics.cellSize, y * SquareMetrics.cellSize, 0);
                // 设置格子对象的缩放比例
                cellObj.transform.localScale = Vector3.one * 0.98f;
                // 将格子对象设置为当前网格管理器的子对象
                cellObj.transform.parent = this.transform;

                // 获取格子对象的渲染器组件
                var renderer = cellObj.GetComponent<Renderer>();
                // 设置格子的材质颜色
                renderer.material.color = new Color(64 / 255f, 0, 64 / 255f);

                // 为格子对象添加SquareCell组件
                var cell = cellObj.AddComponent<SquareCell>();
                // 设置格子的type
                GridType cellType = RandomGridType.GetRandomGridType();
                cell.SetGridType(cellType);
                // 调用Init方法初始化SquareCell组件，传入格子的坐标
                cell.Init(new SquareCoordinates(x, y));
                // 将初始化好的格子存储到二维数组中
                cells[x, y] = cell;

                // 初始化地块的探索状态
                cell.IsExplored = false;
            }
        }

        // 设置所有方向的邻居关系（包括对角线）
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                SquareCell cell = cells[x, y];

                // 基本方向
                SetNeighborIfValid(cell, x, y, SquareDirection.N);
                SetNeighborIfValid(cell, x, y, SquareDirection.S);
                SetNeighborIfValid(cell, x, y, SquareDirection.E);
                SetNeighborIfValid(cell, x, y, SquareDirection.W);

                // 对角线方向
                if (x > 0 && y < height - 1) // 左上
                    SetNeighborIfValid(cell, x, y, SquareDirection.NW);

                if (x < width - 1 && y < height - 1) // 右上
                    SetNeighborIfValid(cell, x, y, SquareDirection.NE);

                if (x > 0 && y > 0) // 左下
                    SetNeighborIfValid(cell, x, y, SquareDirection.SW);

                if (x < width - 1 && y > 0) // 右下
                    SetNeighborIfValid(cell, x, y, SquareDirection.SE);
            }
        }

        // 设置格子的类型，确保相邻格子类型不同时为 BirdSquare
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                SquareCell cell = cells[x, y];
                GridType cellType;
                do
                {
                    cellType = RandomGridType.GetRandomGridType();
                    // 检查已设置类型的邻居
                } while (HasNeighborWithTypeAfterInit(cell, GridType.BirdSquare) && cellType == GridType.BirdSquare);
                cell.SetGridType(cellType);
            }
        }

        // 确保至少有指定数量的 BirdSquare
        EnsureBirdSquares(AllCells.ToList(), requiredCount);

        Debug.Log($"Total Cells Generated: {width * height}");
    }

    // 辅助方法：检查并设置指定方向的邻居
    private void SetNeighborIfValid(SquareCell cell, int x, int y, SquareDirection direction)
    {
        var (dx, dy) = direction.GetOffset();
        int nx = x + dx;
        int ny = y + dy;

        if (nx >= 0 && nx < width && ny >= 0 && ny < height)
        {
            // 确保邻居格子已创建
            if (cells[nx, ny] != null)
            {
                cell.SetNeighbor(direction, cells[nx, ny]);
            }
        }
    }

    /// <summary>
    /// 在邻居关系设置后，检查指定格子是否有指定类型的邻居
    /// </summary>
    private bool HasNeighborWithTypeAfterInit(SquareCell cell, GridType type)
    {
        // 检查基本方向的邻居
        var directions = new[] { SquareDirection.N, SquareDirection.S, SquareDirection.E, SquareDirection.W };
        foreach (var direction in directions)
        {
            if (cell.TryGetNeighbor(direction, out SquareCell neighbor))
            {
                // 检查邻居的类型是否已设置且匹配
                if (neighbor.GetGridType() == type)
                {
                    return true; // 找到匹配类型的邻居
                }
            }
        }
        return false; // 未找到匹配类型的邻居
    }


    // 通过坐标获取格子
    public SquareCell GetCell(int x, int y)
    {
        if (x >= 0 && x < width && y >= 0 && y < height)
            return cells[x, y];
        return null;
    }

    // 通过世界坐标获取格子
    public SquareCell GetCell(Vector3 worldPosition)
    {
        var coord = SquareCoordinates.FromWorldPosition(worldPosition); // 推荐在SquareCoordinates实现此静态方法
        return GetCell(coord.X, coord.Y);
    }

    public Vector3 WorldToGridCoordinates(Vector2 worldPosition)
    {
        var coord = SquareCoordinates.FromWorldPosition(worldPosition); // 推荐在SquareCoordinates实现此静态方法
        return new Vector3(coord.X, coord.Y, 0);
    }
    public Vector3 GridToWorldCoordinates(Vector2 gridPosition)
    {
        return new Vector3(gridPosition.x * SquareMetrics.cellSize, gridPosition.y * SquareMetrics.cellSize, 0);
    }

    /// <summary>
    /// 确保至少有指定数量的 BirdSquare
    /// </summary>
    public void EnsureBirdSquares(List<SquareCell> allCells, int requiredCount)
    {
        // 统计当前已有的 BirdSquare 数量
        int currentCount = 0;
        List<SquareCell> nonBirdCells = new List<SquareCell>();

        foreach (var cell in allCells)
        {
            if (cell.GetGridType() == GridType.BirdSquare)
            {
                currentCount++;
                AllBridCells.Add(cell); // 添加 BirdSquare 格子到集合中
            }
            else
            {
                nonBirdCells.Add(cell); // 仅添加非 BirdSquare 的格子
            }
        }

        // 如果当前数量不足，则随机补充
        int neededCount = requiredCount - currentCount;
        if (neededCount > 0 && nonBirdCells.Count >= neededCount)
        {
            for (int i = 0; i < neededCount; i++)
            {
                int randomIndex = Random.Range(0, nonBirdCells.Count);
                nonBirdCells[randomIndex].SetGridType(GridType.BirdSquare);
                nonBirdCells.RemoveAt(randomIndex); // 确保不会重复选择同一个格子
            }
        }
    }
}
