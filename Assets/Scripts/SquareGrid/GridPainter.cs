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
        if (centerCell is null || centerCell.CellRenderer is null)
        {
            Debug.LogError("Center cell or its renderer is null. Cannot paint area.");
            return;
        }

        // 获取当前格子周围的所有格子
        List<SquareCell> surroundingCells = centerCell.GetSurroundingCells().ToList();

        // 遍历周围的格子并设置它们的颜色
        foreach (SquareCell cell in surroundingCells)
        {
            if (cell.CellRenderer is not null)
            {
                cell.SetColor(Color.white, true); // 设置颜色为白
                if (cell.GetGridType() == GridType.BirdSquare && cell.IsExplored == false)
                {
                    int randomValue = Random.Range(1, 3); // 生成 1 或 2
                                                          // 加载图片资源
                    cell.CellRenderer.sprite = Resources.Load<Sprite>("Images/Bird" + randomValue);
                    Debug.Log("成功为 BirdSquare 设置图片资源。");
                }
                cell.IsExplored = true; // 标记为已探索
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











