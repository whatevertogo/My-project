using CDTU.Utils;
using UnityEngine;


public class HoverManager : Singleton<HoverManager>
{
    private GameObject nowcollider;

    private void Update()
    {
        Vector2 point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(point, Vector2.zero);

        if (hit.collider != null && hit.collider.GetComponent<IHover>() != null)
        {
            // 如果鼠标悬停在物体上，调用 OnHoverEnter 方法
            hit.collider.GetComponent<IHover>()?.OnHoverExit();
            nowcollider = hit.collider.gameObject;
            if (hit.collider != nowcollider)
            {
                nowcollider.GetComponent<IHover>()?.OnHoverEnter();
                nowcollider = hit.collider.gameObject;
            }
        }
    }
}
