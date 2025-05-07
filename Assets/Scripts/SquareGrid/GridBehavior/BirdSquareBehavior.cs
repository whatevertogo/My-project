using UnityEngine;
using System.Collections.Generic;
using HexGame.Harvest;

public class BirdSquareBehavior : IGridTypeBehavior
{
    public void ApplyBehavior(SquareCell cell)
    {
        // 只在格子被探索后添加小鸟
        if (!cell.IsExplored) return;
        
        // 检查是否已经有BirdOverlay子物体，避免重复创建
        if (cell.transform.Find("BirdOverlay") != null) 
        {
            return;
        }
        
        CreateBirdOverlay(cell);
        
        // 如果不存在聊天框，创建一个
        if (!cell.IsCreateChatBox)
        {
            //创建聊天框
            CreateChatBox(cell);
            
            // 使用统一的类获取随机收获类型
            HarvestType wantedType = WantedRandomHarvestType.GetRandomBirdWantedType();
            cell.SetHarvestTypeWanted(wantedType);
            
            // 显示需要的收获物品
            cell.ShowWantedHarvest();
            
            cell.IsCreateChatBox = true;
        }
    }

    public void OnInteract(SquareCell cell)
    {
        // 鸟类格子的交互逻辑
        // 不直接调用OnMouseDown，因为Harvestable没有这个方法
        
        // 检查是否有足够的所需资源
        HarvestType wantedType = cell.GetHarvestTypeWanted();
        if (wantedType != HarvestType.None && HarvestManager.Instance.GetResourceCount(wantedType) > 0)
        {
            // 消耗资源
            HarvestManager.Instance.ConsumeResource(wantedType, 1);
            
            // 更改图像为心形
            if (cell.chatObject != null)
            {
                var harvestImage = cell.chatObject.transform.Find("HarvestImage");
                if (harvestImage != null)
                {
                    harvestImage.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Images/heart");
                }
            }
            
            // 重置需要的物品类型
            cell.SetHarvestTypeWanted(HarvestType.None);
            
            // 得分
            GameManager.Instance.WinCore();
        }
    }

    private void CreateBirdOverlay(SquareCell cell)
    {
        // 创建子物体用于显示小鸟图
        GameObject birdOverlay = new GameObject("BirdOverlay");
        birdOverlay.transform.SetParent(cell.transform, false);
        
        // 设置位置、缩放和旋转
        birdOverlay.transform.localScale = Vector3.one * 0.4f;
        birdOverlay.transform.localPosition = new Vector3(0, 0.4f, -0.1f);
        birdOverlay.transform.localRotation = Quaternion.Euler(-10, 0, 0);
        
        // 添加SpriteRenderer并设置小鸟图
        SpriteRenderer sr = birdOverlay.AddComponent<SpriteRenderer>();
        int randomValue = Random.Range(1, 3); // 生成1或2
        
        sr.sprite = Resources.Load<Sprite>("Images/Bird" + randomValue);
        if (sr.sprite == null)
        {
            Debug.LogWarning("未能加载小鸟图片，使用默认图");
            sr.sprite = Resources.Load<Sprite>("Images/Default");
        }

        // 使用格子的材质
        sr.material = cell.CellRenderer.material;
        sr.sortingLayerName = "Behavior";
        sr.sortingOrder = cell.CellRenderer.sortingOrder + 1; // 确保盖在上面
        
        Debug.Log("在BirdSquare上添加了鸟的贴图。");
    }
    
    private void CreateChatBox(SquareCell cell)
    {
        // 创建对话框
        GameObject chatBox = new GameObject("ChatBox");
        chatBox.transform.SetParent(cell.transform, false);
        
        // 设置位置在小鸟上方
        chatBox.transform.localPosition = new Vector3(0.3f, 0.7f, -0.2f);
        chatBox.transform.localScale = Vector3.one * 0.5f;
        
        // 添加SpriteRenderer并设置对话框图片
        SpriteRenderer sr = chatBox.AddComponent<SpriteRenderer>();
        sr.sprite = Resources.Load<Sprite>("Images/Chat");
        sr.sortingLayerName = "Behavior";
        sr.sortingOrder = 3; // 确保在鸟上方

        // 设置cell的chatObject引用
        cell.chatObject = chatBox;
    }
}