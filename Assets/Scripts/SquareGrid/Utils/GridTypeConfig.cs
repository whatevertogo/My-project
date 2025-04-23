using UnityEngine;

[CreateAssetMenu(fileName = "GridTypeConfig", menuName = "ScriptableObjects/GridTypeConfig", order = 1)]
public class GridTypeConfig : ScriptableObject
{
    [Range(0, 1)]
    public float birdSquareProbability = 0.2f; // 鸟形方格的概率
    public bool ensureAtLeastOneBirdSquare = true; // 是否确保至少有一个鸟形方格

}