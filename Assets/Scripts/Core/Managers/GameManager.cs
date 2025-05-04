using System;
using System.Collections.Generic;
using CDTU.Utils;
using Conclutions;
using HexGame.Harvest;
using UnityEngine;

public class GameManager : SingletonDD<GameManager>
{

    public static ConclutionsType ConclutionType;

    public float time = 0.0f;

    public int core=0;

    public float timeLimit = 3000f; // 5分钟

    // 存储各类资源的数量
    public Dictionary<HarvestType, int> usedCounts = new();


    public void Start()
    {
        // 初始化所有资源类型的计数为0
        foreach (HarvestType type in Enum.GetValues(typeof(HarvestType)))
        {
            usedCounts[type] = 0;
        }
        ConclutionType = ConclutionsType.None;
    }


    public void Update()
    {
        //开始游戏计时
        time += Time.deltaTime;
        if (time > timeLimit)
        {
            // 结束游戏
            CheckConclusion();
            Debug.Log($"游戏结束，结局：{ConclutionType}");

        }
    }

    private void CheckConclusion()
    {
        // 获取资源使用数量
        int branchUsed = usedCounts.ContainsKey(HarvestType.Branch) ? usedCounts[HarvestType.Branch] : 0;
        int pineConeUsed = usedCounts.ContainsKey(HarvestType.PineCone) ? usedCounts[HarvestType.PineCone] : 0;
        int featherUsed = usedCounts.ContainsKey(HarvestType.Feather) ? usedCounts[HarvestType.Feather] : 0;

        // 默认结局
        ConclutionType = ConclutionsType.None;

        // Part1 判断
        if (branchUsed <= 5 && pineConeUsed < 5)
        {
            ConclutionType = ConclutionsType.Part1_WastedResources; // 亲康失败
        }
        else if (branchUsed >= 5 && pineConeUsed < 5)
        {
            ConclutionType = ConclutionsType.Part1_Weak; // 弱小
        }
        else if (branchUsed >= 5 && pineConeUsed >= 5)
        {
            ConclutionType = ConclutionsType.Part1_Warm; // 温暖
        }

        // Part2 判断（米粒判断）
        if (pineConeUsed < 5)
        {
            ConclutionType = ConclutionsType.Part2_Hungry; // 饥饿
        }
        else if (pineConeUsed >= 5)
        {
            ConclutionType = ConclutionsType.Part2_Full; // 吃饱
        }

        // Part3 判断（基于帮助的小鸟数量和资源使用）
        if (core < 5)
        {
            ConclutionType = ConclutionsType.Part3_CompleteNo1; // 亲更弱1
        }
        else if (pineConeUsed < 5)
        {
            ConclutionType = ConclutionsType.Part3_CompleteNo2; // 亲更弱2
        }
        else if (branchUsed >= 5 && pineConeUsed >= 5 && core < 5)
        {
            ConclutionType = ConclutionsType.Part3_Lonely; // 孤零零灵
        }
        else if (branchUsed >= 5 && pineConeUsed >= 5 && core >= 5 && core < 10)
        {
            ConclutionType = ConclutionsType.Part3_GoodFriend; // 好朋友灵
        }
        else if (branchUsed >= 5 && pineConeUsed >= 5 && core >= 10)
        {
            ConclutionType = ConclutionsType.Part3_Home; // 守"星"空
        }

        Debug.Log($"游戏结束，结局：{ConclutionType}");
    }

    /// <summary>
    /// 增加指定类型资源的使用计数
    /// </summary>
    public void AddUsedCount(HarvestType type, int amount = 1)
    {
        if (!usedCounts.ContainsKey(type))
        {
            usedCounts[type] = 0;
        }
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