using UnityEngine;

public class BirdSquareHoverHandler : ICellHoverHandler
{
    private readonly DefaultHoverHandler defaultHandler = new DefaultHoverHandler();

    // 当鼠标悬停在某个SquareCell上时调用的方法
    public void OnHoverEnter(SquareCell cell)
    {
        // 输出日志信息，显示当前悬停的格子坐标，并提示显示小鸟特效
        Debug.Log(message: $"鸟格子 {cell.Coordinates} 悬停，显示小鸟特效！");
        if (!cell.IsCreateChatBox)
        {
            // 对话或者爱心特效
            var chatImage = Resources.Load<Sprite>("Images/Chat");
            if (chatImage is not null)
            {
                Debug.Log("加载对话框图片成功！");
                cell.chatObject = new GameObject("ChatImage");
                cell.chatObject.transform.SetParent(cell.transform, true);
                var spriteRenderer = cell.chatObject.AddComponent<SpriteRenderer>();
                spriteRenderer.sprite = chatImage;
                spriteRenderer.transform.position = cell.GetWorldPosition() + new Vector3(0, 1, -1); // 设置位置
                spriteRenderer.transform.localScale = new Vector3(0.5f, 0.5f, 1); // 设置缩放
                spriteRenderer.transform.rotation = Quaternion.Euler(-10, 0, 0); // 设置旋转
                spriteRenderer.sortingLayerName = "Behavior";
                spriteRenderer.sortingOrder = 4;// 设置渲染顺序
                cell.IsCreateChatBox = true; // 标记为已创建
                // 设置小鸟对话框希望的物体类型
                cell.harvestTypeWanted = WantedRandomHarvestType.GetRandomHarvestType();
                cell.ShowWantedHarvest();
                // cell.
                //todo-更多的视觉效果
            }
            else
            {
                Debug.LogError("加载对话框图片失败！");
            }

            // 自定义逻辑
            defaultHandler.OnHoverEnter(cell); // 调用默认逻辑
        }
        else
        {
            cell.chatObject.SetActive(true); // 显示对话框
        }
    }

    public void OnHoverExit(SquareCell cell)
    {
        Debug.Log($"鸟格子 {cell.Coordinates} 悬停结束！");
        defaultHandler.OnHoverExit(cell); // 调用默认逻辑
        if (cell.IsCreateChatBox)
        {
            cell.chatObject.SetActive(false); // 隐藏对话框
        }
    }
}
