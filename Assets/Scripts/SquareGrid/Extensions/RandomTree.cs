using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class RandomTree
{
    public static readonly List<Sprite> Tree = new()
    {
        Resources.Load<Sprite>("Images/Tree/huaShu1"),
        Resources.Load<Sprite>("Images/Tree/huaShu2"),
        Resources.Load<Sprite>("Images/Tree/huaShu3"),
    };


    public static T RandomElement<T>(this IEnumerable<T> enumerable)
    {
        var list = enumerable.ToList();
        return list[Random.Range(0, list.Count)];
    }
    public static Sprite GetRandomTree()
    {
        return Tree.RandomElement();
    }
}