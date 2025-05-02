using System.Linq;
using UnityEngine;
using CDTU.Utils;
using System.Collections.Generic;
using System.Collections;

public class GridPainter : Singleton<GridPainter>
{
    private PlayerGridComponent playerGridComponent;
    public Renderer currentCellRenderer;

    [Header("迷雾相关配置")]
    public GameObject[] mistPrefabs; // 迷雾预制体
    public float fadeDuration = 1.0f; // 迷雾消失时间
    public float startSpeed = 0f; // 初始速度
    public float acceleration = 1f; // 每秒加速度
    public float misLocalScaleMin = 0.8f;
    public float misLocalScaleMax = 1.2f;

    [Header("泊松分布参数")]
    public float poissonRadius = 0.5f; // 最小距离
    public int poissonSamplesBeforeRejection = 20; // 采样次数

    [Header("调试设置")]
    public bool enableDebugLogs = true; // 控制日志输出
    // 使用字典存储每个格子位置的迷雾对象
    private Dictionary<Vector2Int, List<GameObject>> fogDictionary = new();
    private Transform fogParent; // 迷雾的父物体

    protected override void Awake()
    {
        base.Awake();
        playerGridComponent = GetComponent<PlayerGridComponent>();
        InitializeFogParent(); //初始化迷雾父物体
    }

    private void InitializeFogParent()
    {
        // 检查是否已存在迷雾父物体
        GameObject existingFogParent = GameObject.Find("FogContainer");
        if (existingFogParent is not null)
        {
            fogParent = existingFogParent.transform;
        }
        else
        {
            // 创建新的迷雾父物体
            GameObject fogParentObj = new GameObject("FogContainer");
            fogParent = fogParentObj.transform;
        }
        // 确保迷雾容器在场景根级别
        fogParent.SetParent(null);
    }

    private void Start()
    {
        if (playerGridComponent is not null)
        {
            playerGridComponent.OnCellChanged += OnPlayerCellChanged;
        }
        if (playerGridComponent?.currentCell?.CellRenderer is not null)
        {
            currentCellRenderer = playerGridComponent.currentCell.CellRenderer;
            PaintArea(playerGridComponent.currentCell);
        }
    }

    private void OnPlayerCellChanged(object sender, PlayerGridComponent.OnCellChangedEventArgs e)
    {
        if (e.cell is not null && e.cell.CellRenderer is not null && e.cell.CellRenderer != currentCellRenderer)
        {
            currentCellRenderer = e.cell.CellRenderer;
            if (enableDebugLogs) Debug.Log($"Current cell renderer updated: {currentCellRenderer.name}");
        }

        PaintArea(e.cell);
    }
    /// <summary>
    /// 画小鸟和雾
    /// </summary>
    public void PaintArea(SquareCell centerCell)
    {
        if (centerCell is null || centerCell.CellRenderer is null)
        {
            Debug.LogError("Center cell or its renderer is null. Cannot paint area.");
            return;
        }

        // 清除当前格子及周围的迷雾
        ClearMistAround(centerCell);

        // 获取并处理周围的格子
        List<SquareCell> surroundingCells = centerCell.GetSurroundingCells().ToList();
        foreach (SquareCell cell in surroundingCells)
        {
            // 如果是小鸟格子，添加小鸟贴图
            if (cell.GetGridType() == GridType.BirdSquare)
            {
                // 检查是否已经有 BirdOverlay 子物体，避免重复创建
                if (cell.transform.Find("BirdOverlay") is not null)
                {
                    Debug.Log("BirdOverlay 已存在，跳过创建。");
                    continue;
                }

                int randomValue = Random.Range(1, 3); // 生成 1 或 2

                // 创建子物体用于显示小鸟图
                GameObject birdOverlay = new GameObject("BirdOverlay");
                birdOverlay.transform.SetParent(cell.transform, false);
                // if (randomValue == 1)
                // {
                //     birdOverlay.transform.localPosition = new Vector3(-0.2f, -0.2f, 0); // 可微调位置
                // }
                // else if (randomValue == 2)
                // {
                //     birdOverlay.transform.localPosition = new Vector3(0.2f, -0.2f, 0); // 可微调位置
                // }
                birdOverlay.transform.localScale = Vector3.one * 0.6f; // 缩小一点

                // 添加 SpriteRenderer 并设置小鸟图
                SpriteRenderer sr = birdOverlay.AddComponent<SpriteRenderer>();
                sr.sprite = Resources.Load<Sprite>("Images/Bird" + randomValue);
                if (sr.sprite is null)
                {
                    Debug.LogError("未能加载小鸟图片，跳过设置。");
                    continue;
                }
                sr.material = cell.CellRenderer.material; // 使用格子的unit材质
                birdOverlay.transform.localRotation = Quaternion.Euler(-10, 0, 0);
                birdOverlay.transform.localPosition = new Vector3(0, 0, -0.1f); // 确保在格子上面
                sr.sortingLayerName = "Behavior";
                sr.sortingOrder = cell.CellRenderer.sortingOrder + 1; // 确保盖在上面

                Debug.Log("在 BirdSquare 上添加了鸟的贴图。");
            }

            // 标记为已探索
            cell.IsExplored = true;
        }
    }

    // 清除指定格子位置的迷雾
    private void ClearMistAtCell(Vector2Int cellPos)
    {
        if (!HasFogInDictionary(cellPos)) return;

        var mistsToRemove = fogDictionary[cellPos].ToList();
        foreach (var mist in mistsToRemove)
        {
            if (mist is not null)
            {
                Vector2 currentDir = MistDir(mist);
                StartCoroutine(FadeOut(mist, currentDir));
                RemoveFogFromDictionary(cellPos, mist);
            }
        }
    }

    // 清除当前格子及周围八个格子的迷雾
    private void ClearMistAround(SquareCell centerCell)
    {
        if (centerCell is null) return;

        var centerPos = new Vector2Int(centerCell.Coordinates.X, centerCell.Coordinates.Y);

        // 清除3x3范围内的所有迷雾
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                Vector2Int cellPos = new Vector2Int(centerPos.x + x, centerPos.y + y);
                ClearMistAtCell(cellPos);
            }
        }
    }

    // 获取迷雾移动方向
    private Vector2 MistDir(GameObject mist)
    {
        if (mist is null || playerGridComponent?.currentCell is null)
            return Vector2.up;

        return ((Vector2)(mist.transform.position - playerGridComponent.currentCell.transform.position)).normalized;
    }

    // 迷雾淡出效果
    private IEnumerator FadeOut(GameObject mist, Vector3 dir)
    {
        if (mist is null) yield break;

        float elapsedTime = 0f;
        float speed = startSpeed;
        SpriteRenderer mistSr = mist.GetComponent<SpriteRenderer>();
        if (mistSr is null) yield break;

        Color color = mistSr.color;
        Vector2Int cellPos = new Vector2Int(Mathf.RoundToInt(mist.transform.position.x), Mathf.RoundToInt(mist.transform.position.y));

        while (elapsedTime < fadeDuration && mist is not null)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(color.a, 0f, elapsedTime / fadeDuration);
            mistSr.color = new Color(color.r, color.g, color.b, alpha);

            speed += acceleration * Time.deltaTime;
            mist.transform.position += dir.normalized * speed * Time.deltaTime;
            yield return null;
        }

        if (mist is not null)
        {
            RemoveFogFromDictionary(cellPos, mist);
            Destroy(mist);
        }
    }

    private void OnDestroy()
    {
        if (playerGridComponent is not null)
        {
            playerGridComponent.OnCellChanged -= OnPlayerCellChanged;
        }
    }

    // 为指定位置添加迷雾
    public void AddFogToCell(Vector2Int cellPos, GameObject fog)
    {
        if (!fogDictionary.ContainsKey(cellPos))
        {
            fogDictionary[cellPos] = new List<GameObject>();
        }
        fogDictionary[cellPos].Add(fog);
    }

    // 生成单个格子的迷雾
    public void GenerateMist(int x, int y)
    {
        List<Vector2> fogPoints = GeneratePoissonPoints(poissonRadius, poissonSamplesBeforeRejection);
        Vector2Int cellPos = new Vector2Int(x, y);

        foreach (var pt in fogPoints)
        {
            //todo-也许你会微调他的值
            Vector3 pos = new Vector3(x + pt.x - 0.5f, y + pt.y - 0.5f, -1f);
            GameObject fog = CreateFogObject(pos);
            if (fog is not null)
            {
                AddFogToDictionary(cellPos, fog);
            }
        }
    }

    // 创建迷雾对象
    private GameObject CreateFogObject(Vector3 position)
    {
        GameObject prefab = mistPrefabs[Random.Range(0, mistPrefabs.Length)];
        GameObject fog = Instantiate(prefab, position, Quaternion.identity, fogParent);
        fog.transform.localScale *= Random.Range(misLocalScaleMin, misLocalScaleMax);
        return fog;
    }

    // 网上找的泊松分布
    // numSamplesBeforeRejection用于控制采样次数，次数减少可以减少雾气数量

    /// <summary>
    /// 生成泊松分布的点
    /// </summary>
    /// <param name="radius">最小允许距离</param>
    /// <param name="numSamplesBeforeRejection">控制采样次数</param>
    /// <returns>生成的点列表</returns>
    public List<Vector2> GeneratePoissonPoints(float radius, int numSamplesBeforeRejection = 20)
    {
        List<Vector2> points = new List<Vector2>();// 最终结果
        List<Vector2> spawnPoints = new List<Vector2>();// 当前可以生成新点的“种子点”

        float cellSize = radius / Mathf.Sqrt(2);
        int gridSize = Mathf.CeilToInt(1f / cellSize); // 限制在 1x1 区域内
        Vector2[,] grid = new Vector2[gridSize, gridSize];

        // 初始点中心
        spawnPoints.Add(new Vector2(0.5f, 0.5f));

        while (spawnPoints.Any())
        {
            int spawnIndex = UnityEngine.Random.Range(0, spawnPoints.Count);
            Vector2 spawnCenter = spawnPoints[spawnIndex];
            bool accepted = false;

            for (int i = 0; i < numSamplesBeforeRejection; i++)
            {
                float angle = UnityEngine.Random.value * Mathf.PI * 2;
                float dist = UnityEngine.Random.Range(radius, 2 * radius);
                Vector2 candidate = spawnCenter + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * dist;

                if (candidate.x >= 0 && candidate.x < 1 && candidate.y >= 0 && candidate.y < 1 && IsFarEnough(candidate, points, radius))
                {
                    points.Add(candidate);
                    spawnPoints.Add(candidate);
                    accepted = true;
                    break;
                }
            }

            if (!accepted)
            {
                spawnPoints.RemoveAt(spawnIndex);
            }
        }

        return points;
    }
    /// <summary>
    /// 判断候选点是否与已有所有点之间的距离都大于等于给定的半径。
    /// </summary>
    /// <param name="candidate">候选点</param>
    /// <param name="points">已有点列表</param>
    /// <param name="radius">最小允许距离</param>
    /// <returns>如果候选点与所有已有点的距离都大于等于 radius，则返回 true；否则返回 false</returns>
    private bool IsFarEnough(Vector2 candidate, List<Vector2> points, float radius)
    {
        foreach (var p in points)
        {
            if ((candidate - p).sqrMagnitude < radius * radius)
                return false;
        }
        return true;
    }

    // 为整个网格生成迷雾
    public void GenerateAllMist(int width, int height)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GenerateMist(x, y);
            }
        }
        if (enableDebugLogs) Debug.Log($"已生成 {fogDictionary.Count} 个格子的迷雾");
    }

    // 添加迷雾到字典
    private void AddFogToDictionary(Vector2Int cellPos, GameObject fog)
    {
        if (!HasFogInDictionary(cellPos))
        {
            fogDictionary[cellPos] = new List<GameObject>();
        }
        fogDictionary[cellPos].Add(fog);
    }

    // 从字典中移除迷雾
    private void RemoveFogFromDictionary(Vector2Int cellPos, GameObject fog)
    {
        if (HasFogInDictionary(cellPos))
        {
            fogDictionary[cellPos].Remove(fog);
            if (!fogDictionary[cellPos].Any())
            {
                fogDictionary.Remove(cellPos);
            }
        }
    }
    private bool HasFogInDictionary(Vector2Int cellPos)
    {
        return fogDictionary.ContainsKey(cellPos) && fogDictionary[cellPos].Count > 0;
    }

}
