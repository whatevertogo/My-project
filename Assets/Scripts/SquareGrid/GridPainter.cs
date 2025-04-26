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
            if (cell.CellRenderer is null) continue;

            // 首先标记为已探索
            cell.IsExplored = true;
            
            // 设置颜色为白色（已探索）
            cell.SetColor(Color.white, true);

            // 如果是小鸟格子，添加小鸟贴图
            if (cell.GetGridType() == GridType.BirdSquare)
            {
                // 检查是否已经有 BirdOverlay 子物体，避免重复创建
                if (cell.transform.Find("BirdOverlay") != null)
                {
                    Debug.Log("BirdOverlay 已存在，跳过创建。");
                    continue;
                }

                int randomValue = Random.Range(1, 3); // 生成 1 或 2

                // 创建子物体用于显示小鸟图
                GameObject birdOverlay = new GameObject("BirdOverlay");
                birdOverlay.transform.SetParent(cell.transform, false);
                if (randomValue == 1)
                {
                    birdOverlay.transform.localPosition = new Vector3(-0.2f, -0.2f, 0); // 可微调位置
                }
                else if (randomValue == 2)
                {
                    birdOverlay.transform.localPosition = new Vector3(0.2f, -0.2f, 0); // 可微调位置
                }
                birdOverlay.transform.localScale = Vector3.one * 0.5f; // 缩小一点

                // 添加 SpriteRenderer 并设置小鸟图
                SpriteRenderer sr = birdOverlay.AddComponent<SpriteRenderer>();
                sr.sprite = Resources.Load<Sprite>("Images/Bird" + randomValue);
                if (sr.sprite == null)
                {
                    Debug.LogError("未能加载小鸟图片，跳过设置。");
                    continue;
                }
                sr.sortingOrder = cell.CellRenderer.sortingOrder + 1; // 确保盖在上面

                Debug.Log("在 BirdSquare 上添加了鸟的贴图。");
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











