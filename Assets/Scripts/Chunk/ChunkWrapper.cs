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

        // 绑定渲染纹理到 RenderObject
        chunk.BindRenderToObject();

        // 将 RenderObject 设置为 ChunkWrapper 的子物体
        if (chunk.RenderObject is null) return;
        chunk.RenderObject.transform.SetParent(this.transform,false);
        chunk.RenderObject.transform.localPosition = Vector3.zero;
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