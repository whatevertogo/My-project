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
                renderer.material.color = new Color(64 / 255, 0, 64 / 255);

                // 为格子对象添加SquareCell组件
                var cell = cellObj.AddComponent<SquareCell>();
                // 调用Init方法初始化SquareCell组件，传入格子的坐标
                cell.Init(new SquareCoordinates(x, y)); 
                // 将初始化好的格子存储到二维数组中
                cells[x, y] = cell;
            }
        }
        // 生成后立即设置邻居
        // 遍历网格的每一列
        for (int x = 0; x < width; x++)
        {
            // 遍历网格的每一行
            for (int y = 0; y < height; y++)
            {
                // 获取当前遍历到的格子
                var cell = cells[x, y];
                // 遍历四个方向，假设SquareDirection只有4个方向
                for (int d = 0; d < 4; d++) 
                {
                    // 将整数转换为SquareDirection枚举类型
                    SquareDirection dir = (SquareDirection)d;
                    // 获取该方向的偏移量
                    var (dx, dy) = dir.GetOffset();
                    // 计算邻居格子的x坐标
                    int nx = x + dx;
                    // 计算邻居格子的y坐标
                    int ny = y + dy;
                    // 检查邻居格子的坐标是否在网格范围内
                    if (nx >= 0 && nx < width && ny >= 0 && ny < height)
                    {
                        // 如果在范围内，设置当前格子在该方向的邻居
                        cell.SetNeighbor(dir, cells[nx, ny]);
                    }
                }
            }
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
