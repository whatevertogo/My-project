using UnityEngine;
using HexGame.Harvest;
using DG.Tweening;
public class PlantPineConeEffect : CardEffect
{
    public PlantPineConeEffect(Card card) : base(card){}

    public override bool CanUse(SquareCell targetCell)
    {
        // 只能在空白格子上种树
        return targetCell.GetGridType() == GridType.SimpleSquare;
    }

    public override void Execute(SquareCell targetCell)
    {
        if (!CanUse(targetCell)) return;

        // 将格子转为松树格子
        targetCell.SetGridType(GridType.PlantedTree);
        var cellPineCone = new GameObject("CellPineCone"); // 创建松树对象

        cellPineCone.transform.SetParent(targetCell.transform);
        /* var spriteRenderer = cellPineCone.AddComponent<SpriteRenderer>();

        cellPineCone.transform.localPosition = new Vector3(0, 0.4f, -1);

        cellPineCone.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);

        cellPineCone.transform.localRotation = Quaternion.Euler(-10, 0, 0); // 设置旋转

        spriteRenderer.sprite = Resources.Load<Sprite>("Images/Tree/songShu");

        // spriteRenderer.transform.rotation = Quaternion.Euler(-10, 0, 0); // 设置旋转
        spriteRenderer.sortingLayerName = "Behavior";
        spriteRenderer.sortingOrder = 11; // 设置渲染顺序 */


        var spriteRenderer = cellPineCone.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = Resources.Load<Sprite>("Images/Tree/songShu");
        spriteRenderer.sortingLayerName = "Behavior";
        spriteRenderer.sortingOrder = 11;

        // 设置初始位置和旋转
        cellPineCone.transform.localPosition = new Vector3(0, -0.6f, -1);  // 底部对齐
        cellPineCone.transform.localRotation = Quaternion.Euler(-10, 0, 0); // 轻微倾斜

        // 设置树木的缩放为 0, 只改变Y轴
        cellPineCone.transform.localScale = new Vector3(0.2f, 0f, 0.2f); // Y轴初始为0
        spriteRenderer.transform.localScale = new Vector3(0.2f, 0f, 0.2f);

         // 添加从下往上生长的动画
        cellPineCone.transform.DOKill();  // 停止任何现有的动画
        cellPineCone.transform.DOScaleY(0.2f, 1f)  // Y轴从0增长到0.2
            .SetEase(Ease.OutBounce); // 弹性效果

        // 添加淡入效果
        spriteRenderer.DOFade(0, 0); // 初始透明
        spriteRenderer.DOFade(1, 1f); // 1秒内完全显示

        // 添加轻微摇摆动画
        cellPineCone.transform.DORotate(new Vector3(-10, 0, 10), 0.5f) // 先往右摆
            .SetLoops(2, LoopType.Yoyo) // 往返摇摆一次
            .SetEase(Ease.InOutSine);

        // 添加可收获组件
        Harvestable harvestable = targetCell.gameObject.AddComponent<Harvestable>();
        harvestable.SetResourceType(HarvestType.PineCone);
        harvestable.SetHarvestAmount(1);

        Debug.Log($"在 {targetCell.Coordinates} 种植了松树");

        // 直接移除当前执行的卡牌
        CardManager.Instance.RemoveCard(card);
    }
}