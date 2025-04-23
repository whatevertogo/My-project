using UnityEngine;

public static class MouseTool
{
    /// <summary>
    /// 获取鼠标在世界坐标系中的位置
    /// </summary>
    /// <returns>鼠标在世界坐标系中的位置</returns>
    public static Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Camera.main.nearClipPlane; // 设置z轴为相机的近裁剪面
        return Camera.main.ScreenToWorldPoint(mousePos);
    }

}