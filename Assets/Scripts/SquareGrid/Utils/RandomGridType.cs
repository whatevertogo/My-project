using UnityEngine;
using System.Collections.Generic;

public static class RandomGridType
{
    private static GridTypeConfig config;
    private static bool hasBirdSquare = false; // 标记是否已经生成了 BirdSquare

    /// <summary>
    /// 初始化配置
    /// </summary>
    public static void Initialize(GridTypeConfig gridTypeConfig)
    {
        config = gridTypeConfig;
        hasBirdSquare = false; // 每次初始化时重置标记
    }

    /// <summary>
    /// 随机生成一个网格类型
    /// </summary>
    /// <returns></returns>
    public static GridType GetRandomGridType()
    {
        if (config == null)
        {
            throw new System.Exception("GridTypeConfig 未初始化，请调用 RandomGridType.Initialize() 方法进行初始化！");
        }

        // 正常随机生成
        float r = Random.Range(0f, 1f);
        if (r < config.birdSquareProbability)
        {
            hasBirdSquare = true;
            return GridType.BirdSquare;
        }
        else
        {
            return GridType.SimpleSquare;
        }
    }

    /// <summary>
    /// 确保至少有一个 BirdSquare
    /// </summary>
    public static void EnsureAtLeastOneBirdSquare(List<SquareCell> allCells)
    {
        if (!hasBirdSquare && config.ensureAtLeastOneBirdSquare)
        {
            // 随机选择一个格子并设置为 BirdSquare
            int randomIndex = Random.Range(0, allCells.Count);
            allCells[randomIndex].SetGridType(GridType.BirdSquare);
            hasBirdSquare = true;
        }
    }
}
