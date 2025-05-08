using System.Collections.Generic;
using System.Linq;
using CDTU.Utils;
using UnityEngine;
using System;

public class GridManager : Singleton<GridManager>
{
    [Header("网格大小配置")]
    public readonly int width = 25;
    public readonly int height = 25;

    public Vector3 GetGridCenter() => new Vector3(width / 2, height / 2, 0);
    public int GrassCount = 100;//要生成的草的格子数

    [Header("网格类型配置和需要的BridSquare数量")]
    public GridTypeConfig gridTypeConfig; // 网格类型配置文件
    public SquareCell[,] cells;
    public Material cellMaterial;

    [Header("羽毛刷新相关")]
    public float FeatherTimer = 10;
    public float currentFeatherTime = 0;
    [Tooltip("每次刷新羽毛的数量")]
    public int FeatherCount = 1;

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
        base.Awake();
        RandomGridType.Initialize(gridTypeConfig);
        currentFeatherTime = FeatherTimer;
        //生成网格
        GenerateGrid();
    }

    public void Update()
    {
        if (currentFeatherTime > 0)
        {
            currentFeatherTime -= Time.deltaTime;
        }
        else
        {
            RandomSetFeatherType();
            currentFeatherTime = FeatherTimer;
        }

    }


    /// <summary>
    /// 随机在可用格子中刷新羽毛（Feather）类型
    /// </summary>
    private void RandomSetFeatherType()
    {

        // 1. 过滤出可作为羽毛的格子（这里只允许普通格子）
        var candidates = AllCells
            .Where(cell => cell.GetGridType() == GridType.SimpleSquare && cell.IsPlaceable)
            .ToList();

        // 2. 如果候选格子不足，直接返回
        if (candidates.Count == 0) return;

        // 2. 随机抽取指定数量的格子
        int count = Mathf.Min(FeatherCount, candidates.Count);
        for (int i = 0; i < count; i++)
        {
            // Unity 推荐的洗牌算法：Fisher-Yates
            int randomIndex = UnityEngine.Random.Range(i, candidates.Count);
            (candidates[i], candidates[randomIndex]) = (candidates[randomIndex], candidates[i]);

            candidates[i].SetGridType(GridType.Feather);
        }
    }

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

                // 设置格子对象的位置，使用 Round 确保像素对齐
                float posX = Mathf.Round(x * SquareMetrics.cellSize * 100f) / 100f;
                float posY = Mathf.Round(y * SquareMetrics.cellSize * 100f) / 100f;
                cellObj.transform.position = new Vector3(posX, posY, 0);

                // 使用整体缩放而不是单独缩放
                cellObj.transform.localScale = Vector3.one;

                // 将格子对象设置为当前网格管理器的子对象
                cellObj.transform.parent = this.transform;

                // 添加并配置 SpriteRenderer
                var CellRenderer = cellObj.AddComponent<SpriteRenderer>();

                // 为格子对象添加SquareCell组件
                var cell = cellObj.AddComponent<SquareCell>();
                //设置格子的透明度
                Color A = cell.CellRenderer.color;
                A.a = 0f;
                cell.CellRenderer.color = A;
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

        GridPainter.Instance?.GenerateAllMist(width, height);
        GridPainter.Instance?.GenerateGrass(width, height, GrassCount); // 生成 4 个草地
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
            if (cells[(int)nx, (int)ny] is not null)
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
        var coord = SquareCoordinates.FromWorldPosition(worldPosition);
        return GetCell(coord.X, coord.Y);
    }

    // 通过Coordinates获取格子
    public Vector3 WorldToGridCoordinates(Vector2 worldPosition)
    {
        var coord = SquareCoordinates.FromWorldPosition(worldPosition);
        return new Vector3(coord.X, coord.Y, 0);
    }
    //通过格子坐标系坐标获取世界坐标系坐标
    public Vector3 GridToWorldCoordinates(Vector2 gridPosition)
    {
        return new Vector3(gridPosition.x * SquareMetrics.cellSize, gridPosition.y * SquareMetrics.cellSize, 0);
    }

    private void AssignGridTypes()
    {
        Vector3 centerV3 = GetGridCenter();
        Vector2Int center = new Vector2Int((int)centerV3.x, (int)centerV3.y);
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                SquareCell cell = cells[x, y];
                if (x == center.x && y == center.y)
                {
                    cell.SetGridType(GridType.SimpleSquare);
                    var homeObj = new GameObject("home");
                    homeObj.transform.SetParent(cell.transform);
                    var homeRenderer = homeObj.AddComponent<SpriteRenderer>();
                    homeRenderer.sprite = Resources.Load<Sprite>("Images/Home");
                    homeRenderer.sortingLayerName = "Behavior";
                    homeRenderer.sortingOrder = 3;
                    homeObj.transform.localPosition = new Vector3(0, 0.57f, -1);
                    homeObj.transform.localRotation = Quaternion.Euler(-10, 0, 0);
                    cell.IsPlaceable = false;
                }
                else
                {
                    GridType cellType = RandomGridType.GetRandomGridType();
                    cell.SetGridType(cellType);
                }
            }
        }


        /* 已在中心点创建了家，这部分代码不再需要
        var home = new GameObject("home");
        home.transform.SetParent(cellHome.transform);
        var spriteRenderer = home.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = Resources.Load<Sprite>("Images/Home");
        spriteRenderer.sortingLayerName = "Behavior";
        spriteRenderer.sortingOrder = 3;
        home.transform.localPosition = new Vector3(0, 0.57f, -1);
        home.transform.localRotation = Quaternion.Euler(-10, 0, 0);
        cellHome.IsPlaceable = false; // 设置出生点不可放置
        */
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
