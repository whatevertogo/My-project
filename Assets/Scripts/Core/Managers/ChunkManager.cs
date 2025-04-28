using System.Collections.Generic;
using UnityEngine;

public class ChunkManager : MonoBehaviour
{
    public int chunkSize = 5; // 每个区块的大小
    public int mapWidth = 25; // 地图宽度（格子数）
    public int mapHeight = 25; // 地图高度（格子数）

    private Dictionary<(int, int), List<SquareCell>> chunks = new(); // 存储每个区块的格子

    public void InitializeChunks(SquareCell[,] cells)
    {
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                int chunkX = x / chunkSize;
                int chunkY = y / chunkSize;
                var chunkKey = (chunkX, chunkY);

                if (!chunks.ContainsKey(chunkKey))
                {
                    chunks[chunkKey] = new List<SquareCell>();
                }

                chunks[chunkKey].Add(cells[x, y]);
            }
        }

        Debug.Log($"Total Chunks: {chunks.Count}");
    }

    public List<SquareCell> GetChunk(int chunkX, int chunkY)
    {
        var chunkKey = (chunkX, chunkY);
        if (chunks.ContainsKey(chunkKey))
        {
            return chunks[chunkKey];
        }
        return null;
    }

    public void RenderChunkToTexture(int chunkX, int chunkY, RenderTexture renderTexture)
    {
        var chunk = GetChunk(chunkX, chunkY);
        if (chunk == null)
        {
            Debug.LogWarning($"Chunk ({chunkX}, {chunkY}) not found!");
            return;
        }

        // 创建一个临时摄像机用于渲染
        GameObject tempCameraObj = new GameObject("TempCamera");
        Camera tempCamera = tempCameraObj.AddComponent<Camera>();
        tempCamera.orthographic = true;
        tempCamera.orthographicSize = chunkSize / 2f;
        tempCamera.targetTexture = renderTexture;

        // 计算区块中心点
        Vector3 chunkCenter = CalculateChunkCenter(chunk);
        tempCamera.transform.position = new Vector3(chunkCenter.x, chunkCenter.y, -10);

        // 渲染到 RenderTexture
        tempCamera.Render();

        // 销毁临时摄像机
        Destroy(tempCameraObj);
    }

    private Vector3 CalculateChunkCenter(List<SquareCell> chunk)
    {
        Vector3 center = Vector3.zero;
        foreach (var cell in chunk)
        {
            center += cell.transform.position;
        }
        return center / chunk.Count;
    }
}
