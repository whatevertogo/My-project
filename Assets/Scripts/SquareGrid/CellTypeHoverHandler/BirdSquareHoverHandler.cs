using UnityEngine;

public class BirdSquareHoverHandler : ICellHoverHandler
{
    private readonly DefaultHoverHandler defaultHandler = new DefaultHoverHandler();

    public void OnHoverEnter(SquareCell cell)
    {
        Debug.Log($"鸟格子 {cell.Coordinates} 悬停，显示小鸟特效！");
        // 对话或者爱心特效
        var chatImage = Resources.Load<Sprite>("Images/Chat");
        if (chatImage is not null)
        {
            Debug.Log("加载对话框图片成功！");
            var chatObject = new GameObject("ChatImage");
            var spriteRenderer = chatObject.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = chatImage;
            spriteRenderer.transform.position = cell.GetWorldPosition() + new Vector3(0, 1, 0); // 设置位置
            spriteRenderer.transform.localScale = new Vector3(0.5f, 0.5f, 1); // 设置缩放
            spriteRenderer.sortingOrder = 4;// 设置渲染顺序
            //todo-更多的视觉效果
        }
        else
        {
            Debug.LogError("加载对话框图片失败！");
        }

        // 自定义逻辑
        defaultHandler.OnHoverEnter(cell); // 调用默认逻辑
    }

    public void OnHoverExit(SquareCell cell)
    {
        Debug.Log($"鸟格子 {cell.Coordinates} 悬停结束！");
        defaultHandler.OnHoverExit(cell); // 调用默认逻辑
    }
}
