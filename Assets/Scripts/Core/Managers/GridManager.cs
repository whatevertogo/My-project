using System.Collections.Generic;
using System.Linq;
using CDTU.Utils;
using UnityEngine;
using System;

public class GridManager : Singleton<GridManager>
{
    [Header("网格大小配置")]
    public int width = 25;
    public int height = 25;

    public GameObject[] mistPrefeb;//预制体

    public Vector3 GetGridCenter() => new Vector3(width / 2, height / 2, 0);

    [Header("网格类型配置和需要的BridSquare数量")]
    public GridTypeConfig gridTypeConfig; // 网格类型配置文件
    public int requiredCount = 2; // 至少需要的 BirdSquare 数量
    public SquareCell[,] cells;
    [Header("迷雾相关的配置")]
    public List<GameObject> fogList = new List<GameObject>();
    public int minFogPerTile = 3;//最少生成的迷雾
    public int maxFogPerTile = 5;//最多生成的迷雾
    public IEnumerable<SquareCell> AllCells
    {
        get
        {
            foreach (var cell in cells)
                yield return cell;
        }
    }
    public List<SquareCell> AllBridCells = new();
    public List<SquareCell> AllDontMoveCells = new();

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
                GameObject cellObj = new GameObject();
                // 为格子对象命名，包含其坐标信息
                cellObj.name = $"Cell_{x}_{y}";
                // 设置格子对象的位置，根据其坐标和格子大小计算
                cellObj.transform.position = new Vector3(x * SquareMetrics.cellSize, y * SquareMetrics.cellSize, 0);
                // 设置格子对象的缩放比例
                cellObj.transform.localScale = Vector3.one * 0.98f;
                // 将格子对象设置为当前网格管理器的子对象
                cellObj.transform.parent = this.transform;

                // 添加格子对象的渲染器组件
                cellObj.AddComponent<SpriteRenderer>();

                // 为格子对象添加SquareCell组件
                var cell = cellObj.AddComponent<SquareCell>();
                //设置格子的透明度
                Color A = cell.CellRenderer.color;
                A.a = 0f;
                cell.CellRenderer.color = A;
                GenerateMist(x, y);//放置迷雾
                // 设置格子的type
                cell.SetGridType(RandomGridType.GetRandomGridType());
                //设置格子的碰撞体
                cellObj.AddComponent<BoxCollider2D>();
                // 调用Init方法初始化SquareCell组件，传入格子的坐标
                cell.Init(new SquareCoordinates(x, y));
                // 将初始化好的格子存储到二维数组中
                cells[x, y] = cell;

            }
        }

        // 设置所有方向的邻居关系（包括对角线）
        SetAllNeighbors();

        // 设置格子的类型，确保相邻格子类型不同时为 BirdSquare
        AssignGridTypes();

        // 确保至少有指定数量的 BirdSquare
        EnsureBirdSquares(AllCells.ToList(), requiredCount);

        Debug.Log($"Total Cells Generated: {width * height}");
    }

    // 辅助方法：检查并设置指定方向的邻居
    private void SetNeighborIfValid(SquareCell cell, int x, int y, SquareDirection direction)
    {
        var (dx, dy) = direction.GetOffset();
        float nx = x + dx;
        float ny = y + dy;

        if (nx >= 0 && nx < width && ny >= 0 && ny < height)
        {
            // 确保邻居格子已创建
            if (cells[(int)nx, (int)ny] != null)
            {
                cell.SetNeighbor(direction, cells[(int)nx, (int)ny]);
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

    // 通过Coordinates获取格子
    public Vector3 WorldToGridCoordinates(Vector2 worldPosition)
    {
        var coord = SquareCoordinates.FromWorldPosition(worldPosition); // 推荐在SquareCoordinates实现此静态方法
        return new Vector3(coord.X, coord.Y, 0);
    }
    //通过格子坐标系坐标获取世界坐标系坐标
    public Vector3 GridToWorldCoordinates(Vector2 gridPosition)
    {
        return new Vector3(gridPosition.x * SquareMetrics.cellSize, gridPosition.y * SquareMetrics.cellSize, 0);
    }

    /// <summary>
    /// todo-确保至少有指定数量的 BirdSquare
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
                int randomIndex = UnityEngine.Random.Range(0, nonBirdCells.Count);
                nonBirdCells[randomIndex].SetGridType(GridType.BirdSquare);
                nonBirdCells.RemoveAt(randomIndex); // 确保不会重复选择同一个格子
            }
        }
    }
    //生成雾
    private void GenerateMist(int x, int y)
    {
        List<Vector2> fogPoints = GeneratePoissonPoints(0.5f); // 参数为最小距离，可调

        foreach (var pt in fogPoints)
        {
            Vector3 pos = new Vector3(x + pt.x - 0.5f, y + pt.y - 0.5f, -1f);
            //检查 mistPrefeb 是否为空或长度为 0
            if (mistPrefeb is null || mistPrefeb.Length == 0)
            {
                Debug.LogError("mistPrefeb array is null or empty! Please assign mist prefabs in the Inspector.");
                return;
            }
            GameObject prefab = mistPrefeb[UnityEngine.Random.Range(0, mistPrefeb.Length)];
            GameObject fog = Instantiate(prefab, pos, Quaternion.identity, transform);
            fog.transform.localScale *= UnityEngine.Random.Range(0.8f, 1.2f);
            fogList.Add(fog);
        }
    }
    //网上找的泊松分布
    //numSamplesBeforeRejection用于控制采样次数，次数减少可以减少雾气数量
    public List<Vector2> GeneratePoissonPoints(float radius, int numSamplesBeforeRejection = 20)
    {
        List<Vector2> points = new List<Vector2>();// 最终结果
        List<Vector2> spawnPoints = new List<Vector2>();// 当前可以生成新点的“种子点”

        float cellSize = radius / Mathf.Sqrt(2);
        int gridSize = Mathf.CeilToInt(1f / cellSize); // 限制在 1x1 区域内
        Vector2[,] grid = new Vector2[gridSize, gridSize];

        // 初始点中心
        spawnPoints.Add(new Vector2(0.5f, 0.5f)); 

        while (spawnPoints.Any())
        {
            int spawnIndex = UnityEngine.Random.Range(0, spawnPoints.Count);
            Vector2 spawnCenter = spawnPoints[spawnIndex];
            bool accepted = false;

            for (int i = 0; i < numSamplesBeforeRejection; i++)
            {
                float angle = UnityEngine.Random.value * Mathf.PI * 2;
                float dist = UnityEngine.Random.Range(radius, 2 * radius);
                Vector2 candidate = spawnCenter + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * dist;

                if (candidate.x >= 0 && candidate.x < 1 && candidate.y >= 0 && candidate.y < 1 && IsFarEnough(candidate, points, radius))
                {
                    points.Add(candidate);
                    spawnPoints.Add(candidate);
                    accepted = true;
                    break;
                }
            }

            if (!accepted)
            {
                spawnPoints.RemoveAt(spawnIndex);
            }
        }

        return points;
    }
    bool IsFarEnough(Vector2 candidate, List<Vector2> points, float radius)//泊松分布用于计算距离
    {
        foreach (var p in points)
        {
            if ((candidate - p).sqrMagnitude < radius * radius)
                return false;
        }
        return true;
    }

    // 优化后的格子类型分配逻辑
    private void AssignGridTypes()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                SquareCell cell = cells[x, y];
                GridType cellType = RandomGridType.GetRandomGridType();

                // 确保出生点不是 BirdSquare
                if (GetCell(GetGridCenter()).GetGridType() == GridType.BirdSquare)
                    cellType = GridType.SimpleSquare;

                cell.SetGridType(cellType);

                if (cell.GetGridType() == GridType.BirdSquare)
                {
                    AllBridCells.Add(cell);
                }
            }
        }
    }

    // 提取邻居关系设置为通用方法
    private void SetAllNeighbors()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                SquareCell cell = cells[x, y];
                foreach (SquareDirection direction in Enum.GetValues(typeof(SquareDirection)))
                {
                    SetNeighborIfValid(cell, x, y, direction);
                }
            }
        }
    }
}
