using System.Linq;
using UnityEngine;
using CDTU.Utils;
using System.Collections.Generic;

public class GridPainter : Singleton<GridPainter>
{
    private PlayerGridComponent playerGridComponent;
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

    public void PaintArea(SquareCell centerCell)
    {

    }

    private void OnDestroy()
    {
        if (playerGridComponent != null)
        {
            playerGridComponent.OnCellChanged -= OnPlayerCellChanged;
        }
    }
}











