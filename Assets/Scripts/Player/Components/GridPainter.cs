using System.Linq;
using UnityEngine;
using CDTU.Utils;
using System.Collections.Generic;
using System.Collections;

public class GridPainter : Singleton<GridPainter>
{
    private PlayerGridComponent playerGridComponent;
    public Renderer currentCellRenderer;
    public float fadeDuration = 1.0f; //迷雾消失时间
    public float startSpeed = 0f; // 初始速度
    public float acceleration = 1f; // 每秒加多少速度
    private float currentSpeed; // 实时速度
    private Vector2 moveDirection;//放置移动方向
    public float radius = 1f;//检测驱散的迷雾范围
    List<GameObject> mistList = new List<GameObject>();

    protected override void Awake()
    {
        base.Awake();
        playerGridComponent = GetComponent<PlayerGridComponent>();
    }
    private void Start()
    {
        currentSpeed = startSpeed; // 初始化当前速度
        if (playerGridComponent is not null)
        {
            playerGridComponent.OnCellChanged += OnPlayerCellChanged;
        }
        // 初始化当前格子的渲染器
        if (playerGridComponent?.currentCell?.CellRenderer != null)
        {
            currentCellRenderer = playerGridComponent.currentCell.CellRenderer;
        }
        PaintArea(playerGridComponent.currentCell);
        mistList = GridManager.Instance.fogList;

        // 初始时清除一次迷雾
        ClearMistAround(playerGridComponent.currentCell.transform.position);
    }

    private void OnPlayerCellChanged(object sender, PlayerGridComponent.OnCellChangedEventArgs e)
    {
        if (e.cell is not null && e.cell.CellRenderer is not null && e.cell.CellRenderer != currentCellRenderer)
        {
            currentCellRenderer = e.cell.CellRenderer;
            Debug.Log($"Current cell renderer updated: {currentCellRenderer.name}");
        }

        PaintArea(e.cell);

    }

    private void ClearMistAround(Vector3 position)
    {
        List<GameObject> mistsToRemove = new List<GameObject>();

        foreach (var mist in mistList)
        {
            float distSqr = ((Vector2)(mist.transform.position - position)).sqrMagnitude;
            if (distSqr <= radius * radius)
            {
                Vector2 currentDir = MistDir(mist); // 获取移动方向
                StartCoroutine(FadeOut(mist, currentDir)); // 启用迷雾消散方法
                mistsToRemove.Add(mist);
            }
        }

        // 统一移除迷雾对象
        foreach (var mist in mistsToRemove)
        {
            mistList.Remove(mist);
        }
    }

    public void PaintArea(SquareCell centerCell)
    {
        if (centerCell is null || centerCell.CellRenderer is null)
        {
            Debug.LogError("Center cell or its renderer is null. Cannot paint area.");
            return;
        }

        // 获取当前格子周围的所有格子
        List<SquareCell> surroundingCells = centerCell.GetSurroundingCells().ToList();

        foreach (SquareCell cell in surroundingCells)
        {
            if (cell.CellRenderer is null) continue;

            // 清除迷雾
            ClearMistAround(centerCell.transform.position);

            // 如果是小鸟格子，添加小鸟贴图（逻辑保留）
            if (cell.GetGridType() == GridType.BirdSquare)
            {
                // 检查是否已经有 BirdOverlay 子物体，避免重复创建
                if (cell.transform.Find("BirdOverlay") != null)
                {
                    Debug.Log("BirdOverlay 已存在，跳过创建。");
                    continue;
                }

                int randomValue = Random.Range(1, 3); // 生成 1 或 2

                // 创建子物体用于显示小鸟图
                GameObject birdOverlay = new GameObject("BirdOverlay");
                birdOverlay.transform.SetParent(cell.transform, false);
                if (randomValue == 1)
                {
                    birdOverlay.transform.localPosition = new Vector3(-0.2f, -0.2f, 0); // 可微调位置
                }
                else if (randomValue == 2)
                {
                    birdOverlay.transform.localPosition = new Vector3(0.2f, -0.2f, 0); // 可微调位置
                }
                birdOverlay.transform.localScale = Vector3.one * 0.5f; // 缩小一点

                // 添加 SpriteRenderer 并设置小鸟图
                SpriteRenderer sr = birdOverlay.AddComponent<SpriteRenderer>();
                sr.sprite = Resources.Load<Sprite>("Images/Bird" + randomValue);
                if (sr.sprite is null)
                {
                    Debug.LogError("未能加载小鸟图片，跳过设置。");
                    continue;
                }
                sr.sortingOrder = cell.CellRenderer.sortingOrder + 1; // 确保盖在上面

                Debug.Log("在 BirdSquare 上添加了鸟的贴图。");
            }

            // 最后标记为已探索
            cell.IsExplored = true;
        }
    }

    private void OnDestroy()
    {
        if (playerGridComponent != null)
        {
            playerGridComponent.OnCellChanged -= OnPlayerCellChanged;
        }
    }
    IEnumerator FadeOut(GameObject mist, Vector3 dir)//使迷雾逐渐消失而不是立即消失，并逐渐远离玩家
    {
        
        float elapsedTime = 0f;
        float speed = startSpeed; // 使用局部变量
        SpriteRenderer mistSr = mist.GetComponent<SpriteRenderer>();
        Color color = mistSr.color;//当前的颜色
        while (elapsedTime < fadeDuration)
        {
            //逐渐透明
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(color.a, 0f, elapsedTime / fadeDuration);
            mistSr.color = new Color(color.r, color.g, color.b, alpha);
            //逐渐远离
            speed += acceleration * Time.deltaTime;
            mist.transform.position += dir.normalized * speed * Time.deltaTime;
            yield return null;
        }
        // 最后确保完全透明
        mistSr.color = new Color(color.r, color.g, color.b, 0f);
        Destroy(mist); // 销毁迷雾对象
    }
    
    private Vector2 MistDir(GameObject mist)
    {
        Vector2 direction = (Vector2)(mist.transform.position - playerGridComponent.currentCell.transform.position).normalized;
        Debug.Log("方向是" + direction);
        return direction;
    }
}
