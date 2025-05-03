using CDTU.Utils;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 管理格子悬停和点击交互，兼容带角度的摄像机视角（如 x = -10）
/// </summary>
public class InteractManager : Singleton<InteractManager>
{
    [Header("射线检测配置")]
    [SerializeField] private LayerMask gridLayerMask; // 用于检测格子的层级掩码
    private SquareCell hoveredCell;        // 当前悬停的格子
    private RaycastHit2D currentHit;       // 当前射线检测结果

    protected override void Awake()
    {
        base.Awake();
        // 获取Grid层的LayerMask
        gridLayerMask = LayerMask.GetMask("ChunkCell");
    }

    void Update()
    {
        // 鼠标悬浮在 UI 上不处理
        if (EventSystem.current?.IsPointerOverGameObject() == true)
            return;

        // 获取鼠标在 Z=0 平面上的世界坐标（支持有旋转的摄像机）
        Vector2 point = GetMouseWorldPointOnZ0();

        // 进行物理检测（注意：只检测Grid层的物体）
        currentHit = Physics2D.Raycast(point, Vector2.zero, Mathf.Infinity, gridLayerMask);

        HandleHover();
        HandleClick();
    }

    /// <summary>
    /// 获取鼠标指向 Z=0 平面上的世界坐标，支持带旋转的摄像机
    /// </summary>
    private Vector2 GetMouseWorldPointOnZ0()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        // 射线与 Z=0 平面的交点
        float t = -ray.origin.z / ray.direction.z;
        Vector3 worldPoint = ray.GetPoint(t);

        return new Vector2(worldPoint.x, worldPoint.y);
    }

    /// <summary>
    /// 处理鼠标悬停逻辑
    /// </summary>
    private void HandleHover()
    {
        SquareCell newCell = currentHit.collider?.GetComponent<SquareCell>();

        // 添加调试日志
        Debug.Log($"Hovered Cell: {hoveredCell?.name}, New Cell: {newCell?.name}");

        // 悬停对象发生变化
        if (newCell != hoveredCell)
        {
            // 离开原来的格子
            hoveredCell?.OnHoverExit();

            // 更新悬停的格子
            hoveredCell = newCell;

            // 进入新的格子
            hoveredCell?.OnHoverEnter();
        }
    }

    /// <summary>
    /// 处理点击交互
    /// </summary>
    private void HandleClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            SquareCell clickedCell = currentHit.collider?.GetComponent<SquareCell>();

            if (clickedCell is null) return;
            Debug.Log($"点击到了格子：{clickedCell.name}");
            clickedCell.Interact();
        }
    }

    /// <summary>
    /// 获取当前鼠标下的格子（如果存在）
    /// </summary>
    public SquareCell GetCellUnderMouse()
    {
        return currentHit.collider?.GetComponent<SquareCell>();
    }
}
