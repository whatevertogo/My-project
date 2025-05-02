using UnityEngine;

/// <summary>
/// 用于修复骨骼动画Sprite闪烁问题的组件
/// </summary>
public class SpriteDepthFixer : MonoBehaviour
{
    [Tooltip("Z轴偏移量，用于避免深度冲突")]
    public float zOffset = 0.001f;
    
    private SpriteRenderer[] spriteRenderers;
    
    private void Awake()
    {
        // 获取所有子对象的SpriteRenderer组件
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>(true);
        
        // 确保所有Sprite使用正确的材质设置
        foreach (var renderer in spriteRenderers)
        {
            // 设置材质的渲染模式
            renderer.material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            renderer.material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            
            // 关闭ZWrite以避免深度写入问题
            renderer.material.SetInt("_ZWrite", 0);
            
            // 调整渲染队列
            renderer.material.renderQueue = 3000; // Transparent队列
            
            // 设置sprite的排序层级基于Y轴位置
            renderer.sortingOrder = Mathf.RoundToInt(transform.position.y * -100);
        }
    }

    private void LateUpdate()
    {
        // 动态更新每个Sprite的Z轴位置，避免深度冲突
        foreach (var renderer in spriteRenderers)
        {
            Vector3 position = renderer.transform.localPosition;
            position.z = zOffset * renderer.sortingOrder;
            renderer.transform.localPosition = position;
        }
    }
}
