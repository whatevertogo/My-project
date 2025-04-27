using CDTU.Utils;
using UnityEngine;
using UnityEngine.EventSystems;

public class InteractManager : Singleton<InteractManager>
{
    private SquareCell hoveredCell; // 当前悬停的 SquareCell

    void Update()
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        // 检测鼠标悬停
        Vector2 point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hoverHit = Physics2D.Raycast(point, Vector2.zero);
        if (hoverHit.collider != null)
        {
            SquareCell cell = hoverHit.collider.GetComponent<SquareCell>();
            if (cell != null && cell != hoveredCell && cell.GetHoverHandler() is not DefaultHoverHandler)
            {
                // 如果悬停的格子发生变化
                if (hoveredCell != null)
                {
                    hoveredCell.OnHoverExit(); // 触发之前悬停格子的 OnHoverExit
                }

                hoveredCell = cell;
                hoveredCell.OnHoverEnter(); // 触发当前悬停格子的 OnHoverEnter
            }
        }
        else if (hoveredCell != null)
        {
            // 如果鼠标离开了所有格子
            hoveredCell.OnHoverExit();
            hoveredCell = null;
        }

        // 检测鼠标点击
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D clickHit = Physics2D.Raycast(point, Vector2.zero);
            if (clickHit.collider != null)
            {
                Debug.Log("点击到了：" + clickHit.collider.name);

                SquareCell cell = clickHit.collider.GetComponent<SquareCell>();
                if (cell != null)
                {
                    cell.Interact();
                }
                else
                {
                    Debug.LogWarning("未挂载 SquareCell 脚本！");
                }
            }
        }
    }
}