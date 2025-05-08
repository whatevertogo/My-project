using UnityEngine;
using HexGame.Harvest;
using Unity.IO.LowLevel.Unsafe;
using DG.Tweening;

public class PlantTreeEffect : CardEffect
{
    private int RandomHuaShu = 0;
    public PlantTreeEffect(Card card) : base(card) { }

    public override bool CanUse(SquareCell targetCell)
    {
        // 只能在空白格子上种树
        return targetCell.GetGridType() == GridType.SimpleSquare;
    }
    public override void Execute(SquareCell targetCell)
    {
        if (!CanUse(targetCell)) return;

        if (targetCell.GetGridType() == GridType.PlantedTree)
        {
            Debug.Log("该格子已经种植了树木，无法再次种植。");
            return;
        }

        // 将格子转为树格子
        targetCell.SetGridType(GridType.PlantedTree);

        var CellTree = new GameObject("CellTree");
        CellTree.transform.position = targetCell.transform.position;

        // 将树对象设置为格子对象的子对象
        CellTree.transform.SetParent(targetCell.transform);
        var spriteRenderer = CellTree.AddComponent<SpriteRenderer>();
        RandomHuaShu = Random.Range(0, 2);
        if (RandomHuaShu == 0)
        {
            spriteRenderer.sprite = Resources.Load<Sprite>("Images/Tree/huaShu1");
        }
        else
        {
            spriteRenderer.sprite = Resources.Load<Sprite>("Images/Tree/huaShu2");
        }

        //spriteRenderer.transform.rotation = Quaternion.Euler(-10, 0, 0); // 设置旋转
        spriteRenderer.sortingLayerName = "Behavior";
        spriteRenderer.sortingOrder = 11; // 设置渲染顺序


        // 设置初始位置和旋转
        CellTree.transform.localPosition = new Vector3(0, -0.6f, -1);  // 底部对齐
        CellTree.transform.localRotation = Quaternion.Euler(-10, 0, 0); // 轻微倾斜

        // 设置树木的缩放为 0, 只改变Y轴
        CellTree.transform.localScale = new Vector3(0.2f, 0f, 0.2f); // Y轴初始为0
        spriteRenderer.transform.localScale = new Vector3(0.2f, 0f, 0.2f);

        // 添加从下往上生长的动画
        CellTree.transform.DOKill();  // 停止任何现有的动画
        CellTree.transform.DOScaleY(0.2f, 1f)  // Y轴从0增长到0.2
            .SetEase(Ease.OutBounce); // 弹性效果

        // 添加淡入效果
        spriteRenderer.DOFade(0, 0); // 初始透明
        spriteRenderer.DOFade(1, 1f); // 1秒内完全显示

        int randomRotation = Random.Range(0, 2) * 2 - 1;
        float rotationAngle = randomRotation * 5;
        // 添加轻微摇摆动画
        CellTree.transform.DORotate(new Vector3(-10, 0, randomRotation), 0.5f) // 先往右摆
            .SetLoops(2, LoopType.Yoyo) // 往返摇摆一次
            .SetEase(Ease.InOutSine);


        // 添加可收获组件
        Harvestable harvestable = targetCell.gameObject.AddComponent<Harvestable>();
        targetCell.SetHarvestable(harvestable);
        harvestable.SetResourceType(HarvestType.Branch);
        harvestable.SetHarvestAmount(1);

        Debug.Log($"在 {targetCell.Coordinates} 种植了树");

        // 直接移除当前执行的卡牌
        CardManager.Instance.RemoveCard(card);
    }
}