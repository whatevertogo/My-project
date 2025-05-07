using UnityEngine;

[CreateAssetMenu(fileName = "GridTypeConfig", menuName = "ScriptableObjects/GridTypeConfig", order = 1)]
public class GridTypeConfig : ScriptableObject
{
    [Range(0, 1)]
    public float birdSquareProbability = 0.03f; // 鸟形方格的概率
}