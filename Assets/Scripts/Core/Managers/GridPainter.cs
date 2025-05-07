using System.Linq;
using UnityEngine;
using CDTU.Utils;
using System.Collections.Generic;
using System.Collections;

public class GridPainter : Singleton<GridPainter>
{
    private PlayerGridComponent playerGridComponent;
    [ReadOnly] public Renderer currentCellRenderer;

    [Header("迷雾相关配置")] public GameObject[] mistPrefabs; // 迷雾预制体
    public float fadeDuration = 1.0f; // 迷雾消失时间
    public float startSpeed = 0f; // 初始速度
    public float acceleration = 1f; // 每秒加速度
    public float misLocalScaleMin = 0.8f;
    public float misLocalScaleMax = 1.2f;

    [Header("雾泊松分布参数")] public float poissonRadius = 0.5f; // 雾最最小距离
    public int poissonSamplesBeforeRejection = 20; // 采样次数

    [Header("草地")] public float grassDensity = 0.5f; // 草地密度
    public int grassSamplesBeforeRejection = 20; // 草地采样次数
    public GameObject[] grassPrefabs; // 草地预制体

    [Header("调试设置")] public bool enableDebugLogs = true; // 控制日志输出

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
        if (centerCell is null || centerCell.CellRenderer is null) return;

        // 清除当前格子及周围的迷雾
        ClearMistAround(centerCell);

        // 获取并处理周围的格子
        List<SquareCell> surroundingCells = centerCell.GetSurroundingCells().ToList();
        foreach (SquareCell cell in surroundingCells)
        {
            // 检查是否之前未探索
            bool wasExplored = cell.IsExplored;
            
            // 标记为已探索
            cell.IsExplored = true;
            
            // 如果探索状态改变，重新应用行为（适用于所有类型的格子）
            if (!wasExplored)
            {
                cell.SetGridType(cell.GetGridType());  // 重新应用当前类型的行为
            }
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
        Vector2Int cellPos = new Vector2Int(Mathf.RoundToInt(mist.transform.position.x),
            Mathf.RoundToInt(mist.transform.position.y));

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(color.a, 0f, elapsedTime / fadeDuration);
            mistSr.color = new Color(color.r, color.g, color.b, alpha);

            speed += acceleration * Time.deltaTime;
            mist.transform.position += dir.normalized * speed * Time.deltaTime;
            yield return null;
        }

        RemoveFogFromDictionary(cellPos, mist);
        Destroy(mist);
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
            Vector3 pos = new Vector3(x + pt.x - 0.5f, y + pt.y-0.3f - 0.5f, -1f);
            GameObject fog = CreateFogObject(pos);
            if (fog is not null)
            {
                AddFogToDictionary(cellPos, fog);
            }
        }
    }

    public void GenerateGrass(int width, int height, int grassCount)
    {    //-----随机筛选不重复的格子用于生成草地
        int maxCount = width * height;
        if (grassCount > maxCount)
        {
            Debug.LogError("请求的坐标数超过了范围内所有可能组合！");
        }

        List<int[]> allGridPos = new List<int[]>();
        for (int x = 0; x < width; x++)//生成一个包括了所有位置的数组
        {
            for (int y = 0; y < height; y++)
            {
                allGridPos.Add(new int[] { x, y });
            }
        }

        // 2. 打乱数组
        for (int i = 0; i < allGridPos.Count; i++)
        {
            int j = UnityEngine.Random.Range(i, allGridPos.Count);
            var temp = allGridPos[i];
            allGridPos[i] = allGridPos[j];
            allGridPos[j] = temp;
        }

        // 3. 截取前 grassCount 个作为不重复的随机格子
        var selectedPos = allGridPos.GetRange(0, grassCount);
        //-----------

        var GrassAll = new GameObject("GrassAll");
        for (int i = 0; i < grassCount; i++)
        {
            int x = selectedPos[i][0];
            int y = selectedPos[i][1];

            // 在选定的格子中生成泊松分布的草地
            List<Vector2> grassPoints = GeneratePoissonPoints(grassDensity, grassSamplesBeforeRejection);

            foreach (var pt in grassPoints)
            {
                // 随机生成草地位置
                Vector3 pos = new Vector3(x + pt.x - 0.5f, y + pt.y - 0.5f, -1f);
                GameObject grass = CreateGrassObject(pos);
                if (grass is not null)
                {
                    grass.transform.SetParent(GrassAll.transform, false); // 将草地对象设置为当前对象的子物体
                }
            }
        }
    }

    private GameObject CreateGrassObject(Vector3 position)
    {
        GameObject prefab = grassPrefabs[Random.Range(0, grassPrefabs.Length)]; // 使用现有的草地预制体
        GameObject grass = Instantiate(prefab, position, Quaternion.identity);
        grass.transform.localScale *= Random.Range(misLocalScaleMin, misLocalScaleMax);
        return grass;
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
    // numSamplesBeforeRejection用于控制采样次数，次数减少可以减少雾气数量,反之
    /// <param name="radius">最小允许距离</param>
    /// <param name="numSamplesBeforeRejection">控制采样次数</param>
    /// <returns>生成的点列表</returns>
    public List<Vector2> GeneratePoissonPoints(float radius, int numSamplesBeforeRejection = 20)
    {
        List<Vector2> points = new List<Vector2>(); // 最终结果
        List<Vector2> spawnPoints = new List<Vector2>(); // 当前可以生成新点的"种子点"

        float cellSize = radius / Mathf.Sqrt(2);
        int gridSize = Mathf.CeilToInt(1f / cellSize); // 限制在 1x1 区域内

        // 初始点中心
        spawnPoints.Add(new Vector2(0.5f, 0.5f));

        while (spawnPoints.Any())
        {
            int spawnIndex = Random.Range(0, spawnPoints.Count);
            Vector2 spawnCenter = spawnPoints[spawnIndex];
            bool accepted = false;

            for (int i = 0; i < numSamplesBeforeRejection; i++)
            {
                float angle = Random.value * Mathf.PI * 2;
                float dist = Random.Range(radius, 2 * radius);
                Vector2 candidate = spawnCenter + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * dist;

                if (candidate.x is >= 0 and < 1 && candidate.y is >= 0 and < 1 &&
                    IsFarEnough(candidate, points, radius))
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