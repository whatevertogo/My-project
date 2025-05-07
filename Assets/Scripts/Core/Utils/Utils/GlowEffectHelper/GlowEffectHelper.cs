using UnityEngine;

public static class GlowEffectHelper
{
    private static readonly string GlowShaderName = "Custom/SpriteGlow";

    // 添加发光
    public static void ApplyGlow(SpriteRenderer sr, Color glowColor)
    {
        if (sr == null || sr.sprite == null)
        {
            Debug.LogWarning("SpriteRenderer 或 Sprite 为空，无法添加发光效果。");
            return;
        }

        // 防止共用材质导致多个对象一起亮
        if (sr.material == null || sr.sharedMaterial.name != "InstancedGlowMat")
        {
            Material mat = new Material(Shader.Find(GlowShaderName));
            mat.name = "InstancedGlowMat";
            sr.material = mat;
        }
        

        sr.material.SetColor("_EmissionColor", new Color(glowColor.r, glowColor.g, glowColor.b, 1f));
    }

    // 移除发光（置空发光颜色）
    public static void RemoveGlow(SpriteRenderer sr)
    {
        if (sr != null && sr.material != null && sr.material.HasProperty("_EmissionColor"))
        {
            sr.material.SetColor("_EmissionColor", new Color(0, 0, 0, 0));
        }
    }
}
