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

    private void PaintArea(SquareCell centerCell)
    {
        if (centerCell == null) return;

        // 获取九宫格范围内所有有效的格子
        var validCells = centerCell.GetNineGridCells();

        // 处理每个有效的格子
        foreach (SquareCell cell in validCells)
        {
            if (!activatedCells.Contains(cell))
            {
                activatedCells.Add(cell);

                // 检查地块是否已探索
                if (!cell.IsExplored)
                {
                    //todo-颜色修改 
                    cell.SetColor(Color.black, true); // 未探索地块显示为黑色
                    continue;
                }
                //todo-颜色修改
                cell.SetColor(Color.white, true); // 已探索地块显示为白色
            }
        }
    }

    private void OnPlayerCellChanged(object sender, PlayerGridComponent.OnCellChangedEventArgs e)
    {
        if (e.cell is not null && e.cell.CellRenderer is not null && e.cell.CellRenderer != currentCellRenderer)
        {
            currentCellRenderer = e.cell.CellRenderer;
            Debug.Log($"Current cell renderer updated: {currentCellRenderer.name}");
        }

        // 标记当前地块为已探索
        if (e.cell != null)
        {
            e.cell.IsExplored = true;
        }

        PaintArea(e.cell);
        
    }

    private void OnDestroy()
    {
        if (playerGridComponent != null)
        {
            playerGridComponent.OnCellChanged -= OnPlayerCellChanged;
        }
    }
}











