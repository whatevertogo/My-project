using System.Net.NetworkInformation;
using UnityEngine;

public class ClickManager : MonoBehaviour
{
    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            SquareCell cell = hit.collider.GetComponent<SquareCell>();
            if (cell != null)
            {
                // 检测鼠标悬停在格子上
                if (cell.IsExplored)
                {
                    cell.OnHoverEnter(); // 调用悬停逻辑
                }
            }
            else
            {
                cell.OnHoverExit();
            }
            if (Input.GetMouseButtonDown(0)) // 检测鼠标左键点击
            {
                cell.Interact(); // 调用交互逻辑
            }
        }
    }
}