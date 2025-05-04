using System;
using System.Collections.Generic;
using CDTU.Utils;
using Conclutions;
using HexGame.Harvest;
using UnityEngine;

public class GameManager : SingletonDD<GameManager>
{

    public static ConclutionsPart1Type ConclutionPart1Type = ConclutionsPart1Type.None;

    public static ConclutionsPart2Type ConclutionPart2Type = ConclutionsPart2Type.None;

    public static ConclutionsPart3Type ConclutionPart3Type = ConclutionsPart3Type.None;

    public float time = 0.0f;

    public int core = 0;

    public float timeLimit = 3000f; // 5分钟

    // 存储各类资源的数量
    public Dictionary<HarvestType, int> usedCounts = new();
    private int EndCount = 0;//用于计算能否进入结局part3


    public void Start()
    {
        // 初始化所有资源类型的计数为0
        foreach (HarvestType type in Enum.GetValues(typeof(HarvestType)))
        {
            usedCounts[type] = 0;
        }
        EndCount = 0; // 保证结局参数为0
    }


    public void Update()
    {
        //开始游戏计时
        time += Time.deltaTime;
        if (time > timeLimit)
        {
            // 结束游戏
            CheckConclusion();
            Debug.Log($"游戏结束，结局：{ConclutionPart1Type}");

        }
    }

    private void CheckConclusion()
    {
        // 获取资源使用数量
        int branchUsed = usedCounts.ContainsKey(HarvestType.Branch) ? usedCounts[HarvestType.Branch] : 0;
        int pineConeUsed = usedCounts.ContainsKey(HarvestType.PineCone) ? usedCounts[HarvestType.PineCone] : 0;
        int featherUsed = usedCounts.ContainsKey(HarvestType.Feather) ? usedCounts[HarvestType.Feather] : 0;

        // Part1 判断（同时判断树枝和羽毛）
        if (branchUsed < 5 || featherUsed < 5)
        {
            ConclutionPart1Type = ConclutionsPart1Type.Part1_Wasted; //筑巢失败
        }
        else if (branchUsed >= 5 && featherUsed >= 5)
        {
            ConclutionPart1Type = ConclutionsPart1Type.Part1_Home; // 筑巢成功
            EndCount++;
        }

        // Part2 判断（米粒判断）
        if (pineConeUsed < 5)
        {
            ConclutionPart2Type = ConclutionsPart2Type.Part2_Hungry; // 饥饿
        }
        else if (pineConeUsed >= 5)
        {
            ConclutionPart2Type = ConclutionsPart2Type.Part2_Full; // 吃饱
            EndCount++;
        }

        // Part3 判断（基于帮助的小鸟数量和资源使用）
        if (EndCount < 2 || core < 5)
        {
            ConclutionPart3Type = ConclutionsPart3Type.Part3_Lonely; // 孤零零灵
        }
        else if (EndCount == 2 && core >= 5)
        {
            ConclutionPart3Type = ConclutionsPart3Type.Part3_GoodFriend; // 好朋友灵
        }

        Debug.Log($"游戏结束，结局：{ConclutionPart1Type},{ConclutionPart2Type},{ConclutionPart3Type}");
    }

    /// <summary>
    /// 增加指定类型资源的使用计数
    /// </summary>
    public void AddUsedCount(HarvestType type, int amount = 1)
    {
        usedCounts.TryAdd(type, 1);
        usedCounts[type] += amount;
        Debug.Log($"使用了 {amount} 个 {type}，总共使用：{usedCounts[type]}");
    }

    /// <summary>
    /// 当成功帮助一个小鸟时调用
    /// </summary>
    public void WinCore()
    {
        core++;
        Debug.Log($"帮助了第 {core} 只小鸟！");
    }
}