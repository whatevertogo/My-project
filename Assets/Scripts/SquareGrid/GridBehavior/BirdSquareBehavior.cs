using UnityEngine;
using System.Collections.Generic;
using HexGame.Harvest;
using Spine.Unity;
using Spine;

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
        // 检查是否有足够的所需资源
        HarvestType wantedType = cell.GetHarvestTypeWanted();
        if (wantedType != HarvestType.None && HarvestManager.Instance.GetResourceCount(wantedType) > 0)
        {
            // 消耗资源
            HarvestManager.Instance.ConsumeResource(wantedType, 1);

            // 更改图像为心形
            if (cell.chatObject is not null)
            {
                var harvestImage = cell.chatObject.transform.Find("HarvestImage");
                if (harvestImage is not null)
                {
                    harvestImage.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Images/heart");
                }
            }

            // 重置需要的物品类型
            cell.SetHarvestTypeWanted(HarvestType.None); //  将格子需要的收获类型设置为None，表示已经满足需求


            // 得分
            GameManager.Instance.WinCore();
            CreateScorePopup(cell); // 创建得分弹窗
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

        // SpriteRenderer sr = birdOverlay.AddComponent<SpriteRenderer>();
        // int randomValue = Random.Range(1, 3); // 生成1或2

        // sr.sprite = Resources.Load<Sprite>("Images/Bird" + randomValue);

        // 添加动画
        var birdAnimation = birdOverlay.AddComponent<SkeletonAnimation>();

        // 初始设置为动画1
        birdAnimation.skeletonDataAsset = Resources.Load<SkeletonDataAsset>("Birds/YaQue/ReferenceAssets/shanQue_Standby1");
        //birdAnimation.AnimationState.Complete += AnimationCompleteHandler;

        // 添加动画控制器组件
        BirdAnimationController animController = birdOverlay.AddComponent<BirdAnimationController>();
        animController.Initialize(birdAnimation);

        // 使用格子的材质
        // sr.material = cell.CellRenderer.material;
        // sr.sortingLayerName = "Behavior";
        // sr.sortingOrder = cell.CellRenderer.sortingOrder + 1; // 确保盖在上面

        Debug.Log("在BirdSquare上添加了鸟的动画。");
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

    private void AnimationCompleteHandler(TrackEntry trackEntry)
    {
        // 获取完成的动画名称
        string animationName = trackEntry?.Animation?.Name;

        // 如果没有动画名称，直接返回
        if (string.IsNullOrEmpty(animationName))
            return;

        Debug.Log($"动画 {animationName} 已完成播放");

        // 如果有需要，可以在这里添加特定动画完成后的逻辑
        // 例如，根据不同的动画名称执行不同的操作
        if (animationName.Contains("Standby2"))
        {
            // 可以在这里添加动画2完成后的特殊逻辑
            // 注意：如果BirdAnimationController已经在处理动画序列，
            // 这里可能不需要额外的逻辑
        }
    }


    private void CreateScorePopup(SquareCell cell)
    {
        var scorePopup = new GameObject("ScorePopup");
        var spriteRenderer = scorePopup.AddComponent<SpriteRenderer>();
        scorePopup.transform.SetParent(cell.transform, false);
        spriteRenderer.sprite = Resources.Load<Sprite>("Images/ScorePopup");
        // var prefab = Resources.Load<GameObject>("Prefabs/ScorePopup");// 加载预制件没时间写了要睡觉好困
        spriteRenderer.sortingLayerName = "Behavior";
        spriteRenderer.sortingOrder = 5; // 确保在鸟和对话框上方
                                         // scorePopup.transform.localPosition = new Vector3(0, 0.5f, -0.1f);
        
    }
}