using UnityEngine;

public class BridSquareInitializer : GridInitializer
{
    public override void Init(GridType gridType, Renderer renderer)
    {
        // 确保 Renderer 不为空
        if (renderer == null)
        {
            Debug.LogError("Renderer 为空，无法初始化 BirdSquare 图片！");
            return;
        }

        // 检查地块是否已探索
        SquareCell cell = renderer.gameObject.GetComponent<SquareCell>();
        if (cell != null && !cell.IsExplored)
        {
            // 设置默认颜色（例如黑色）
            renderer.material.color = Color.black;
            return;
        }

        // 随机选择图片资源
        int randomIndex = Random.Range(1, 3); // 生成 1 或 2
        string texturePath = $"Images/Brid{randomIndex}";

        // 加载图片资源
        Texture2D birdTexture = Resources.Load<Texture2D>(texturePath);
        if (birdTexture == null)
        {
            Debug.LogError($"未找到 BirdSquare 的图片资源! 路径: {texturePath}");
            return;
        }

        // 将图片设置为材质的主纹理
        renderer.material.mainTexture = birdTexture;
        Debug.Log("成功为 BirdSquare 设置图片资源。");
    }
}