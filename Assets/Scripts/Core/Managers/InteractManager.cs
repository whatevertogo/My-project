using CDTU.Utils;
using UnityEngine;
using UnityEngine.EventSystems;

public class InteractManager : Singleton<InteractManager>
{
    private SquareCell hoveredCell; // 当前悬停的 SquareCell

    private RaycastHit2D hoverHit;

    void Update()
    {
        if (EventSystem.current?.IsPointerOverGameObject() == true) return;

        Vector2 point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (Camera.main.orthographic)
        {
            hoverHit = Physics2D.Raycast(point, Vector2.zero);
        }
        else if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit))
        {
            hoverHit = Physics2D.Raycast(point, Vector2.zero);
        }

        SquareCell cell = hoverHit.collider?.GetComponent<SquareCell>();
        if (cell is not null && cell != hoveredCell && cell.GetHoverHandler() is not DefaultHoverHandler)
        {
            hoveredCell?.OnHoverExit();
            hoveredCell = cell;
            hoveredCell.OnHoverEnter();
        }
        else if (hoveredCell is not null && hoverHit.collider == null)
        {
            hoveredCell.OnHoverExit();
            hoveredCell = null;
        }

        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D clickHit = Physics2D.Raycast(point, Vector2.zero);
            SquareCell clickedCell = clickHit.collider?.GetComponent<SquareCell>();
            if (clickedCell is not null)
            {
                Debug.Log("点击到了：" + clickHit.collider.name);
                clickedCell.Interact();
            }
            else if (clickHit.collider is not null)
            {
                Debug.LogWarning("未挂载 SquareCell 脚本！");
            }
        }
    }
    public SquareCell GetCellUnderMouse()
    {
        Vector2 point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(point, Vector2.zero);

        // 确保碰撞体存在
        if (hit.collider is not null)
        {
            // 获取并检查SquareCell组件
            var cell = hit.collider.GetComponent<SquareCell>();
            if (cell is not null)
            {
                Debug.Log($"鼠标悬停在格子上：{hit.collider.name}");
                return cell;
            }
            else
            {
                Debug.LogWarning($"物体 {hit.collider.name} 上没有SquareCell组件");
            }
        }
        

        return null;
    }
}