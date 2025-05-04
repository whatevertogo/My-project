using UnityEngine;

namespace Conclutions
{
    public enum ConclutionsType
    {
        None,
        // Part1
        Part1_WastedResources,    // 亲康失败：网铲数≤5, 棕花数<5
        Part1_Weak,              // 网铲数5≤n, 棕花数n<5
        Part1_Warm,              // 温暖：网铲数5≤n, 棕花数5≤n

        // Part2
        Part2_Hungry,            // 饥饿：米粒数n<5
        Part2_Full,              // 吃饱：米粒数5≤n

        // Part3
        Part3_CompleteNo1,       // 亲更弱1：n<5
        Part3_CompleteNo2,       // 亲更弱2：米粒数n<5
        Part3_Lonely,            // 孤零零灵：网铲数5≤n, 米粒数5≤n, n<5
        Part3_GoodFriend,        // 好朋友灵：网铲数5≤n, 米粒数5≤n, 5≤n
        Part3_Home               // 守"星"空：网铲数5≤n, 米粒数5≤n, 10≤n
    }
}
