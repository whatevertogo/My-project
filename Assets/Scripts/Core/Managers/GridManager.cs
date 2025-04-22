using System.Collections.Generic;
using CDTU.Utils;
using UnityEngine;

public class GridManager : Singleton<GridManager>
{
    public int width = 10;
    public int height = 10;

    public Vector3 GetGridCenter() => new Vector3(width / 2, height / 2, 0);

    private SquareCell[,] cells;
    public IEnumerable<SquareCell> AllCells
    {
        get
        {
            foreach (var cell in cells)
                yield return cell;
        }
    }

    protected override void Awake()
    {
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
                // 设置格子的随机type
                cell.SetGridType(RandomGridType.GetRandomGridType());
                // 调用Init方法初始化SquareCell组件，传入格子的坐标
                cell.Init(new SquareCoordinates(x, y));
                // 将初始化好的格子存储到二维数组中
                cells[x, y] = cell;
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
                    cell.SetNeighbor(SquareDirection.NW, cells[x - 1, y + 1]);

                if (x < width - 1 && y < height - 1) // 右上
                    cell.SetNeighbor(SquareDirection.NE, cells[x + 1, y + 1]);

                if (x > 0 && y > 0) // 左下
                    cell.SetNeighbor(SquareDirection.SW, cells[x - 1, y - 1]);

                if (x < width - 1 && y > 0) // 右下
                    cell.SetNeighbor(SquareDirection.SE, cells[x + 1, y - 1]);
            }
        }
    }

    // 辅助方法：检查并设置指定方向的邻居
    private void SetNeighborIfValid(SquareCell cell, int x, int y, SquareDirection direction)
    {
        var (dx, dy) = direction.GetOffset();
        int nx = x + dx;
        int ny = y + dy;

        if (nx >= 0 && nx < width && ny >= 0 && ny < height)
        {
            cell.SetNeighbor(direction, cells[nx, ny]);
        }
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
}
