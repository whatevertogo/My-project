using System.Collections.Generic;
using CDTU.Utils;
using UnityEngine;

public class GridManager : Singleton<GridManager>
{
    public int width = 10;
    public int height = 10;

    public Dictionary<SquareCoordinates, SquareMetrics> SquareCoordinates_SquareMetrics;

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

    // 生成网格并设置邻居
    public void GenerateGrid()
    {
        cells = new SquareCell[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GameObject cellObj = GameObject.CreatePrimitive(PrimitiveType.Quad);
                cellObj.name = $"Cell_{x}_{y}";
                cellObj.transform.position = new Vector3(x * SquareMetrics.cellSize, y * SquareMetrics.cellSize, 0);
                cellObj.transform.localScale = Vector3.one * 0.98f;
                cellObj.transform.parent = this.transform;

                var renderer = cellObj.GetComponent<Renderer>();
                renderer.material.color = new Color(64 / 255, 0, 64 / 255);

                var cell = cellObj.AddComponent<SquareCell>();
                cell.Init(new SquareCoordinates(x, y)); // 只用Init方法初始化
                cells[x, y] = cell;
            }
        }
        // 生成后立即设置邻居
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var cell = cells[x, y];
                for (int d = 0; d < 4; d++) // 假设SquareDirection只有4个方向
                {
                    SquareDirection dir = (SquareDirection)d;
                    var (dx, dy) = dir.GetOffset();
                    int nx = x + dx;
                    int ny = y + dy;
                    if (nx >= 0 && nx < width && ny >= 0 && ny < height)
                    {
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
}
