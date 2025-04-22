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
        if (r < 0.98f)
            return GridType.SimpleSquare;
        else
            return GridType.BirdSquare;
        throw new System.Exception("随机生成的网格类型不在预期范围内！");
    }
}
