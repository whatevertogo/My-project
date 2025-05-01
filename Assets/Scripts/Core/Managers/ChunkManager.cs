using System.Collections.Generic;
using UnityEngine;
using CDTU.Utils;

public class ChunkManager : Singleton<ChunkManager>
{
    [Header("区块设置")]
    public readonly int chunkSize = 5; // 每个区块的大小
    [ReadOnly]
    private int mapWidth; // 地图宽度（格子数）
    [ReadOnly]
    private int mapHeight; // 地图高度（格子数）

    [Header("调试用宝宝")]
    public GameObject chunkWrapperPrefab;

    private Dictionary<(int, int), Chunk> chunks = new();
    private static Camera sharedCamera; // 共享的持久摄像机
    [Tooltip("地面精灵")]
    [SerializeField] private Sprite DiBanSprite;

    protected override void Awake()
    {
        base.Awake();
        mapWidth = GridManager.Instance?.width ?? 0;
        mapHeight = GridManager.Instance?.height ?? 0;
    }

    private void OnDestroy()
    {
        // 清理共享摄像机
        if (sharedCamera is not null)
        {
            Destroy(sharedCamera.gameObject);
            sharedCamera = null;
        }
    }

    void Start()
    {
        InitializeChunks(GridManager.Instance.cells);
        //todo- 可视化所有区块（调试用）
        VisualizeAllChunks(chunkWrapperPrefab);
    }

    // 初始化区块，将地图的所有格子划分为区块
    public void InitializeChunks(SquareCell[,] cells)
    {
        chunks.Clear();
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                int chunkX = x / chunkSize;
                int chunkY = y / chunkSize;
                var chunkKey = (chunkX, chunkY);

                if (!chunks.ContainsKey(chunkKey))
                {
                    chunks[chunkKey] = new Chunk(chunkX, chunkY, chunkSize);
                }
                chunks[chunkKey].AddCell(cells[x, y]);
            }
        }

        // 初始化所有区块的渲染
        foreach (var chunk in chunks.Values)
        {
            chunk.InitializeRenderTexture();
            chunk.RefreshRenderTexture();
            chunk.BindRenderToObject();
            chunk.SetChunkSprite(DiBanSprite);
        }

        Debug.Log($"Total Chunks: {chunks.Count}");
    }

    /// <summary>
    /// 获取指定区块
    /// </summary>
    public Chunk GetChunk(int chunkX, int chunkY)
    {
        var chunkKey = (chunkX, chunkY);
        return chunks.TryGetValue(chunkKey, out var chunk) ? chunk : null;
    }

    /// <summary>
    /// 获取指定世界坐标所在的区块
    /// </summary>
    public Chunk GetChunkByWorldPosition(Vector3 worldPosition)
    {
        int chunkX = Mathf.FloorToInt(worldPosition.x / chunkSize);
        int chunkY = Mathf.FloorToInt(worldPosition.y / chunkSize);
        return GetChunk(chunkX, chunkY);
    }

    /// <summary>
    /// 更新指定区块的内容
    /// </summary>
    public void UpdateChunkContent(int chunkX, int chunkY, GameObject content)
    {
        var chunk = GetChunk(chunkX, chunkY);
        if (chunk is null)
        {
            Debug.LogWarning($"Chunk ({chunkX}, {chunkY}) not found!");
            return;
        }

        foreach (var cell in chunk.Cells)
        {
            if (cell.transform.childCount == 0)
            {
                Instantiate(content, cell.transform.position, Quaternion.identity, cell.transform);
            }
            else
            {
                var existingContent = cell.transform.GetChild(0).gameObject;
                existingContent.name = content.name;
                if (existingContent.TryGetComponent<Renderer>(out var renderer) &&
                    content.TryGetComponent<Renderer>(out var contentRenderer))
                {
                    renderer.material.color = contentRenderer.material.color;
                }
            }
        }

        // 更新区块渲染
        chunk.RefreshRenderTexture();
    }

    /// <summary>
    /// 刷新指定区块
    /// </summary>
    public void RefreshChunk(int chunkX, int chunkY)
    {
        var chunk = GetChunk(chunkX, chunkY);
        if (chunk is not null)
        {
            chunk.RefreshRenderTexture();
            chunk.BindRenderToObject();
        }
        else
        {
            Debug.LogWarning($"Chunk ({chunkX}, {chunkY}) not found!");
        }
    }

    /// <summary>
    /// 设置所有区块的可见性
    /// </summary>
    public void SetAllChunksVisible(bool visible)
    {
        foreach (var chunk in chunks.Values)
        {
            chunk.SetVisible(visible);
        }
    }

    // 刷新所有区块
    public void RefreshAllChunks()
    {
        foreach (var chunk in chunks.Values)
        {
            chunk.RefreshRenderTexture();
            chunk.BindRenderToObject();
        }
    }

    // 清理所有区块
    public void ClearAllChunks()
    {
        foreach (var chunk in chunks.Values)
        {
            if (chunk.RenderObject is not null)
            {
                Destroy(chunk.RenderObject);
            }
            if (chunk.RenderTexture is not null)
            {
                chunk.RenderTexture.Release();
                Destroy(chunk.RenderTexture);
            }
        }
        chunks.Clear();
    }

    /// <summary>
    /// 为指定区块设置一张大图片
    /// </summary>
    /// <param name="chunkX">区块的 X 坐标</param>
    /// <param name="chunkY">区块的 Y 坐标</param>
    /// <param name="sprite">要设置的图片</param>
    public void SetChunkImage(int chunkX, int chunkY, Sprite sprite)
    {
        var chunk = GetChunk(chunkX, chunkY);
        if (chunk is null)
        {
            Debug.LogWarning($"Chunk ({chunkX}, {chunkY}) not found!");
            return;
        }

        chunk.SetChunkSprite(sprite);
    }

    /// <summary>
    /// 清除指定区块的图片
    /// </summary>
    /// <param name="chunkX">区块的 X 坐标</param>
    /// <param name="chunkY">区块的 Y 坐标</param>
    public void ClearChunkImage(int chunkX, int chunkY)
    {
        var chunk = GetChunk(chunkX, chunkY);
        if (chunk is null)
        {
            Debug.LogWarning($"Chunk ({chunkX}, {chunkY}) not found!");
            return;
        }

        chunk.ClearChunkSprite();
    }

    /// <summary>
    /// 在场景中可视化所有区块
    /// </summary>
    /// <param name="chunkWrapperPrefab">包含 ChunkWrapper 的预制体</param>
    public void VisualizeAllChunks(GameObject chunkWrapperPrefab)
    {
        foreach (var chunk in chunks.Values)
        {
            // 创建一个 ChunkWrapper 实例
            GameObject wrapperObject = Instantiate(chunkWrapperPrefab, Vector3.zero, Quaternion.identity, transform);
            ChunkWrapper wrapper = wrapperObject.GetComponent<ChunkWrapper>();

            // 初始化 ChunkWrapper
            wrapper.Initialize(chunk);

            // 设置位置为区块的中心
            wrapper.transform.position = chunk.CalculateCenter();
        }
    }
}
