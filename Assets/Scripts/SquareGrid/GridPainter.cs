using UnityEngine;
using System.Collections.Generic;
using CDTU.Utils;
public class GridPainter : Singleton<GridPainter>
{
    private PlayerGridComponent playerGridComponent;
    private HashSet<SquareCell> activatedCells = new HashSet<SquareCell>();
    public Renderer currentCellRenderer;

    protected override void Awake()
    {
        base.Awake();
        playerGridComponent = GetComponent<PlayerGridComponent>();
    }
    private void Start()
    {
        if (playerGridComponent is not null)
        {
            playerGridComponent.OnCellChanged += OnPlayerCellChanged;
        }
        // 初始化hexMesh，避免Update每帧查找
        if (playerGridComponent is not null && playerGridComponent.currentCell is not null)
        {
            currentCellRenderer = playerGridComponent.currentCell.CellRenderer;
        }
    }

    private void OnPlayerCellChanged(object sender, PlayerGridComponent.OnCellChangedEventArgs e)
    {
        // 若当前cell的HexMesh发生变化，及时更新引用
        if (e.cell is not null && e.cell.CellRenderer is not null && e.cell.CellRenderer != currentCellRenderer)
        {
            currentCellRenderer = e.cell.CellRenderer;
        }
        PaintArea(e.cell);
    }

    private void PaintArea(SquareCell centerCell)
    {
        if (centerCell == null) return;
        // 向前4格，左右各1格，后方不变色
        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = 0; dy < 3; dy++) // dy=0为当前位置，1~2为前方
            {
                int x = centerCell.Coordinates.X + dx;
                int y = centerCell.Coordinates.Y + dy;
                SquareCell cell = GridManager.Instance.GetCell(x, y);
                if (cell != null && !activatedCells.Contains(cell))
                {
                    // 可扩展：如cell.IsObstacle等属性判断
                    cell.SetColor(Color.white, true);
                    activatedCells.Add(cell);
                }
            }
        }
    }

    private void OnDestroy()
    {
        if (playerGridComponent != null)
        {
            playerGridComponent.OnCellChanged -= OnPlayerCellChanged;
        }
    }
}











