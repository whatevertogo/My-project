using UnityEngine;

public static class RandomGridType
{
    /// <summary>
    /// 随机生成一个网格类型
    /// </summary>
    /// <returns></returns>
    public static GridType GetRandomGridType()
    {
        float r = Random.Range(0, 1);
        if (r < 0.5f) return GridType.SimpleSquare;
        if (r < 0.8f) return GridType.BirdSquare;
        if (r < 1f) return GridType.TreeSquare;
        throw new System.Exception("随机生成的网格类型不在预期范围内！");
    }
}
