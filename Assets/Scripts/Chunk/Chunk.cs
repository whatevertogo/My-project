using System.Collections.Generic;
using UnityEngine;

public class Chunk
{
    public int ChunkX { get; private set; }
    public int ChunkY { get; private set; }
    public List<SquareCell> Cells { get; private set; } = new List<SquareCell>();

    public GameObject RenderObject { get; private set; }
    public bool IsVisible { get; private set; } = true;
    public Material SharedMaterial { get; private set; } = ChunkManager.Instance.Material;

    private int chunkSize;

    public Chunk(int chunkX, int chunkY, int chunkSize)
    {
        this.ChunkX = chunkX;
        this.ChunkY = chunkY;
        this.chunkSize = chunkSize;
    }

    // 添加格子到区块中
    public void AddCell(SquareCell cell)
    {
        Cells.Add(cell);
        cell.gameObject.layer = LayerMask.NameToLayer("ChunkCell");
    }

    // 为格子设置精灵图片
    public void SetCellSprite(SquareCell cell, Sprite sprite)
    {
        var spriteRenderer = cell.gameObject.GetComponent<SpriteRenderer>();
        if (spriteRenderer is null)
        {
            spriteRenderer = cell.gameObject.AddComponent<SpriteRenderer>();
        }
        spriteRenderer.sprite = sprite;
        spriteRenderer.sortingLayerName = "ChunkCell";
    }

    /// <summary>
    /// 设置区块的可见性
    /// </summary>
    public void SetVisible(bool visible)
    {
        IsVisible = visible;
        if (RenderObject is not null)
        {
            RenderObject.SetActive(visible);
        }
    }

    /// <summary>
    /// 计算区块的中心位置
    /// </summary>
    public Vector3 CalculateCenter()
    {
        Vector3 center = Vector3.zero;
        foreach (var cell in Cells)
        {
            center += cell.transform.position;
        }
        return center / Cells.Count;
    }

    /// <summary>
    /// 初始化渲染对象
    /// </summary>
    public void BindRenderToObject()
    {
        if (RenderObject is null)
        {
            // 创建用于显示的游戏对象
            RenderObject = new GameObject($"ChunkRender_{ChunkX}_{ChunkY}");
            var spriteRenderer = RenderObject.AddComponent<SpriteRenderer>();
            spriteRenderer.sortingOrder = 3;
            spriteRenderer.material = SharedMaterial;
            spriteRenderer.maskInteraction = SpriteMaskInteraction.None; // 避免mask带来的精度问题

            // 计算正确的位置和大小
            Vector3 center = CalculateCenter();
            RenderObject.transform.position = center;

            // 设置大小以匹配区块实际大小
            float actualSize = chunkSize;
            RenderObject.transform.localScale = new Vector3(actualSize, actualSize, 1);

            // 确保渲染对象在正确的层
            RenderObject.layer = LayerMask.NameToLayer("Default");
        }
    }

    // 更新区块中所有格子的图片
    public void UpdateAllCellSprites(Sprite sprite)
    {
        foreach (var cell in Cells)
        {
            SetCellSprite(cell, sprite);
        }
    }

    // 清理资源
    public void Cleanup()
    {
        if (SharedMaterial is not null && SharedMaterial != ChunkManager.Instance.Material)
        {
            GameObject.Destroy(SharedMaterial);
            SharedMaterial = null;
        }

        if (RenderObject is not null)
        {
            GameObject.Destroy(RenderObject);
        }
    }

    /// <summary>
    /// 为整个区块设置一张大图片
    /// </summary>
    public void SetChunkSprite(Sprite sprite)
    {
        if (RenderObject is null)
        {
            Debug.LogWarning($"RenderObject for Chunk ({ChunkX}, {ChunkY}) is not initialized.");
            return;
        }

        var renderer = RenderObject.GetComponent<SpriteRenderer>();
        if (renderer is null)
        {
            Debug.LogError("RenderObject does not have a SpriteRenderer component.");
            return;
        }

        renderer.sprite = sprite;
        renderer.sortingLayerName = "ChunkCell";
    }

    /// <summary>
    /// 清除区块的图片
    /// </summary>
    public void ClearChunkSprite()
    {
        if (RenderObject is null)
        {
            Debug.LogWarning($"RenderObject for Chunk ({ChunkX}, {ChunkY}) is not initialized.");
            return;
        }

        var renderer = RenderObject.GetComponent<SpriteRenderer>();
        if (renderer is not null)
        {
            renderer.sprite = null;
        }
    }
}