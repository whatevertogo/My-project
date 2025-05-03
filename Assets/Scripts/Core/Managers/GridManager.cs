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

    [Header("网格类型配置和需要的BridSquare数量")]
    public GridTypeConfig gridTypeConfig; // 网格类型配置文件
    public SquareCell[,] cells;
    public Material cellMaterial;
    public int GrassCount = 100;

    public IEnumerable<SquareCell> AllCells
    {
        get
        {
            foreach (var cell in cells)
                yield return cell;
        }
    }

    public List<SquareCell> AllBirdCells = new();
    public List<SquareCell> AllDontMoveCells = new();

    protected override void Awake()
    {
        base.Awake();
        RandomGridType.Initialize(gridTypeConfig);
        //生成网格
        GenerateGrid();
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
