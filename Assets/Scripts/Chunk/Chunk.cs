using System.Collections.Generic;
using UnityEngine;

public class Chunk
{
    public int ChunkX { get; private set; }
    public int ChunkY { get; private set; }
    public List<SquareCell> Cells { get; private set; } = new List<SquareCell>();

    public RenderTexture RenderTexture { get; private set; }
    public GameObject RenderObject { get; private set; } // RenderTexture 的物体（可选）
    public bool IsVisible { get; private set; } = true;

    private int chunkSize; // 区块大小（格子数）

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
        // 设置格子的层为 ChunkCell，这样摄像机才能渲染到
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
        spriteRenderer.sortingLayerName = "ChunkCell"; // 确保在正确的排序层
    }

    // 初始化渲染纹理
    public void InitializeRenderTexture()
    {
        if (RenderTexture is null)
        {
            // 创建一个新的渲染纹理，设置更高的分辨率以获得更好的质量
            RenderTexture = new RenderTexture(1024, 1024, 0)
            {
                filterMode = FilterMode.Bilinear, // 使用双线性过滤提高质量
                antiAliasing = 4 // 启用抗锯齿
            };
            RenderTexture.Create();
        }
    }

    /// <summary>
    /// 刷新渲染纹理
    /// </summary>
    public void RefreshRenderTexture()
    {
        if (RenderTexture == null) InitializeRenderTexture();

        // 使用临时摄像机来渲染
        var tempCameraObj = new GameObject($"TempCamera_Chunk({ChunkX},{ChunkY})");
        var camera = tempCameraObj.AddComponent<Camera>();

        // 设置摄像机参数
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = Color.clear; // 使用透明背景
        camera.orthographic = true;
        camera.orthographicSize = chunkSize / 2f;
        camera.targetTexture = RenderTexture;
        camera.cullingMask = LayerMask.GetMask("ChunkCell"); // 只渲染ChunkCell层

        // 设置摄像机位置和旋转
        Vector3 center = CalculateCenter();
        camera.transform.position = new Vector3(center.x, center.y, -10);
        camera.transform.rotation = Quaternion.identity;

        // 渲染
        camera.Render();

        // 销毁临时摄像机
        GameObject.DestroyImmediate(tempCameraObj);
    }

    /// <summary>
    /// 设置区块的可见性
    /// </summary>
    /// <param name="visible"></param>
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
    /// <returns></returns>
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
    /// 绑定渲染纹理到一个物体上
    /// </summary>
    public void BindRenderToObject()
    {
        if (RenderObject is null)
        {
            // 创建用于显示渲染结果的四边形
            RenderObject = new GameObject($"ChunkRender_{ChunkX}_{ChunkY}");
            var spriteRenderer = RenderObject.AddComponent<SpriteRenderer>();
            spriteRenderer.sortingOrder = 3;
            RenderObject.name = $"ChunkRender_{ChunkX}_{ChunkY}";

            // 计算正确的位置和大小
            Vector3 center = CalculateCenter();
            RenderObject.transform.position = center;

            // 设置大小以匹配区块实际大小
            float actualSize = chunkSize;
            RenderObject.transform.localScale = new Vector3(actualSize, actualSize, 1);

            // 创建并设置材质
            var mat = new Material(Shader.Find("Sprites/Default")) // 使用支持精灵的shader
            {
                mainTexture = RenderTexture
            };
            spriteRenderer.material = mat;

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
        RefreshRenderTexture();
    }

    // 清理资源
    public void Cleanup()
    {
        if (RenderTexture is not null)
        {
            RenderTexture.Release();
            GameObject.Destroy(RenderTexture);
        }

        if (RenderObject is not null)
        {
            GameObject.Destroy(RenderObject);
        }
    }

    /// <summary>
    /// 为整个区块设置一张大图片
    /// </summary>
    /// <param name="sprite">要设置的图片</param>
    public void SetChunkSprite(Sprite sprite)
    {
        if (RenderObject is null)
        {
            Debug.LogWarning($"RenderObject for Chunk ({ChunkX}, {ChunkY}) is not initialized.");
            return;
        }

        // 获取或创建材质
        var renderer = RenderObject.GetComponent<SpriteRenderer>();
        if (renderer is null)
        {
            Debug.LogError("RenderObject does not have a SpriteRenderer component.");
            return;
        }

        if (renderer.material is null)
        {
            renderer.material = new Material(Shader.Find("Sprites/Default"));
        }

        //设置照片
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

        var renderer = RenderObject.GetComponent<MeshRenderer>();
        if (renderer is not null && renderer.material is not null)
        {
            renderer.material.mainTexture = null;
        }
    }
}