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
        if (cell != null && cell != hoveredCell && cell.GetHoverHandler() is not DefaultHoverHandler)
        {
            hoveredCell?.OnHoverExit();
            hoveredCell = cell;
            hoveredCell.OnHoverEnter();
        }
        else if (hoveredCell != null && hoverHit.collider == null)
        {
            hoveredCell.OnHoverExit();
            hoveredCell = null;
        }

        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D clickHit = Physics2D.Raycast(point, Vector2.zero);
            SquareCell clickedCell = clickHit.collider?.GetComponent<SquareCell>();
            if (clickedCell != null)
            {
                Debug.Log("点击到了：" + clickHit.collider.name);
                clickedCell.Interact();
            }
            else if (clickHit.collider != null)
            {
                Debug.LogWarning("未挂载 SquareCell 脚本！");
            }
        }
    }

    public SquareCell GetCellUnderMouse()
    {
        Vector2 point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(point, Vector2.zero);
        if (hit.collider is not null)
        {
            Debug.Log("鼠标现在：" + hit.collider?.name);
            return hit.collider.GetComponent<SquareCell>();
        }
        RaycastHit2D[] hits = Physics2D.RaycastAll(point, Vector2.zero);
        return null;
    }
}