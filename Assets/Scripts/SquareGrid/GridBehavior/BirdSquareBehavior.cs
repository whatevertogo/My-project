using UnityEngine;
using HexGame.Harvest;

public class BirdSquareBehavior : IGridTypeBehavior
{

    public void ApplyBehavior(SquareCell cell)
    {
        GridManager.Instance.AllDontMoveCells.Add(cell);
        // 只在格子被探索后添加小鸟
        if (!cell.IsExplored) return;

        // 检查是否已经有BirdOverlay子物体，避免重复创建
        if (cell.transform.Find("BirdOverlay") is not null) return;

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

        // 添加SpriteRenderer并设置小鸟图片
        SpriteRenderer sr = birdOverlay.AddComponent<SpriteRenderer>();
        int randomValue = Random.Range(1, 3); // 生成1或2

        AnimationClip animClipA = null;
        AnimationClip animClipB = null;

        // 根据随机值选择不同的小鸟资源
        if (randomValue == 1)
        {
            sr.sprite =Resources.Load<Sprite>("Animation/zaoZeQue-1-yaQue-shanQue-Standby_00");
            // 为第一种小鸟加载动画片段 (请确保路径和文件名正确)
            animClipA = Resources.Load<AnimationClip>("Animation/try");
            animClipB = Resources.Load<AnimationClip>("Animation/try2");
        }
        else // randomValue == 2
        {
            sr.sprite =Resources.Load<Sprite>("Animation/yaQue-yaQue-shanQue-Standby_000");
            // 为第二种小鸟加载动画片段 (请确保路径和文件名正确)
            animClipA = Resources.Load<AnimationClip>("Animation/YaQue_standby");
            animClipB = Resources.Load<AnimationClip>("Animation/YaQue_standby1");
        }

        // 检查动画片段是否成功加载
        if (animClipA == null || animClipB == null)
        {
            Debug.LogError($"未能为小鸟类型 {randomValue} 加载动画片段。请检查 Resources/Animations/ 文件夹下的文件名和路径。例如: Bird{randomValue}_AnimA.anim 和 Bird{randomValue}_AnimB.anim");
        }
        else
        {
            // 添加并初始化动画播放器组件
            BirdAnimationPlayer player = birdOverlay.AddComponent<BirdAnimationPlayer>();
            player.Initialize(animClipA, animClipB, 8f); // 8秒延迟
        }

        // 使用 Unity 默认的 Sprite 材质
        sr.material = new Material(Shader.Find("Sprites/Default"));
        sr.sortingLayerName = "Behavior";
        sr.sortingOrder = cell.CellRenderer.sortingOrder + 1; // 确保盖在上面

        Debug.Log($"在BirdSquare上为小鸟 (类型 {randomValue}) 添加了动画播放器。");
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

    private void PlayDefaultAnimation(GameObject gameObject)
    {
        gameObject.AddComponent<Animation>();



    }
}