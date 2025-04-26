using CDTU.Utils;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickManager : Singleton<ClickManager>
{
    void Update()
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        if (Input.GetMouseButtonDown(0))
        {
            Vector2 point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(point, Vector2.zero);
            if (hit.collider != null)
            {
                Debug.Log("点击到了：" + hit.collider.name);

                SquareCell cell = hit.collider.GetComponent<SquareCell>();
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