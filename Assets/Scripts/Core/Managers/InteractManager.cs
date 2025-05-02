using CDTU.Utils;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 管理格子悬停和点击交互，兼容带角度的摄像机视角（如 x = -10）
/// </summary>
public class InteractManager : Singleton<InteractManager>
{
    private SquareCell hoveredCell;        // 当前悬停的格子
    private RaycastHit2D currentHit;       // 当前射线检测结果

    void Update()
    {
        // 鼠标悬浮在 UI 上不处理
        if (EventSystem.current?.IsPointerOverGameObject() == true)
            return;

        // 获取鼠标在 Z=0 平面上的世界坐标（支持有旋转的摄像机）
        Vector2 point = GetMouseWorldPointOnZ0();

        // 进行物理检测（注意：只能检测启用了 Collider2D 的物体）
        currentHit = Physics2D.Raycast(point, Vector2.zero);

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
        bool isHoverable = newCell is not null && newCell.GetHoverHandler() is not DefaultHoverHandler;

        // 悬停对象发生变化
        if (newCell != hoveredCell)
        {
            // 离开原来的格子
            hoveredCell?.OnHoverExit();

            // 进入新的格子（如果是合法的）
            hoveredCell = isHoverable ? newCell : null;

            if (hoveredCell != null)
                hoveredCell.OnHoverEnter();
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

            if (clickedCell != null)
            {
                Debug.Log($"点击到了格子：{clickedCell.name}");
                clickedCell.Interact();
            }
            else if (currentHit.collider != null)
            {
                Debug.LogWarning($"点击到了 {currentHit.collider.name}，但没有 SquareCell 组件！");
            }
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
