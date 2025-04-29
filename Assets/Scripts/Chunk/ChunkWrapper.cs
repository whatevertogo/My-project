using UnityEditor;
using UnityEngine;

public class ChunkWrapper : MonoBehaviour
{
    public int ChunkX;
    public int ChunkY;
    public int ChunkSize;
    public int CellCount;

    private Chunk chunk;

    public void Initialize(Chunk chunk)
    {
        this.chunk = chunk;
        ChunkX = chunk.ChunkX;
        ChunkY = chunk.ChunkY;
        ChunkSize = chunk.Cells.Count;
        CellCount = chunk.Cells.Count;
    }

    private void OnDrawGizmos()
    {
        // 可视化区块的边界
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, new Vector3(ChunkSize, ChunkSize, 0));

        // 在场景中显示区块的坐标
        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.white;
        Handles.Label(transform.position, $"Chunk ({ChunkX}, {ChunkY})", style);
    }
}