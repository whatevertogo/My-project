using System.Linq;
using UnityEngine;
using CDTU.Utils;
using System.Collections.Generic;

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
        PaintArea(playerGridComponent.currentCell);
    }

    private void OnPlayerCellChanged(object sender, PlayerGridComponent.OnCellChangedEventArgs e)
    {
        if (e.cell is not null && e.cell.CellRenderer is not null && e.cell.CellRenderer != currentCellRenderer)
        {
            currentCellRenderer = e.cell.CellRenderer;
            Debug.Log($"Current cell renderer updated: {currentCellRenderer.name}");
        }
        PaintArea(e.cell);
        
    }

    private void PaintArea(SquareCell centerCell)
    {
        if (centerCell == null) return;
        
        // 获取九宫格范围内所有有效的格子，无需手动处理空值
        var validCells = centerCell.GetNineGridCells();
        
        // 处理每个有效的格子
        foreach (var cell in validCells)
        {
            if (!activatedCells.Contains(cell))
            {
                activatedCells.Add(cell);
                cell.SetColor(Color.white, true);
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











