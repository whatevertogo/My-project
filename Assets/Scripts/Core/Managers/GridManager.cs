using UnityEngine;
using System.Collections.Generic;

public class GridManager : MonoBehaviour
{
    public int width = 10;
    public int height = 10;
    public SquareCell[,] cells;

    void Awake()
    {
        GenerateGrid();
        SetAllNeighbors();
    }

    // 生成网格
    private void GenerateGrid()
    {
        cells = new SquareCell[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                cells[x, y] = new SquareCell(x, y);
            }
        }
    }

    // 设置所有格子的邻居
    private void SetAllNeighbors()
    {
        foreach (var cell in cells)
        {
            int x = cell.Coordinates.X;
            int y = cell.Coordinates.Y;
            foreach (SquareDirection dir in System.Enum.GetValues(typeof(SquareDirection)))
            {
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
        int x = Mathf.RoundToInt(worldPosition.x * Square.InverseSideLength);
        int y = Mathf.RoundToInt(worldPosition.y * Square.InverseSideLength);
        return GetCell(x, y);
    }
}
