using UnityEngine;
using HexGame.Harvest;

public class PlantedFeatherBehavior : IGridTypeBehavior
{
    public static readonly Sprite featherSprite = Resources.Load<Sprite>("Images/Feather");
    public void ApplyBehavior(SquareCell cell)
    {
        cell.IsPlaceable = false;
        GridManager.Instance.AllDontMoveCells.Add(cell);
        var feather = new GameObject("Feather");
        var featherRenderer = feather.AddComponent<SpriteRenderer>();
        feather.transform.SetParent(cell.transform);
        feather.transform.localPosition = new Vector3(0, 0, 0);
        feather.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
        featherRenderer.sortingLayerName = "Behavior";
        featherRenderer.sprite = featherSprite;
    }

    public void OnInteract(SquareCell cell)
    {
        // 羽毛格子的交互逻辑
        // 收集羽毛并重置为普通格子
        HarvestManager.Instance.AddHarvest(HarvestType.Feather, 1);// 收集羽毛
        cell.SetGridType(GridType.SimpleSquare);
        cell.gridTypeBehavior = new SimpleSquareBehavior(); // 清除格子行为
        GridManager.Instance.AllDontMoveCells.Remove(cell);

        // 移除羽毛对象
        var feather = cell.transform.Find("Feather");
        if (feather == null)
        {
            Debug.Log("没有找到羽毛对象，可能已经被销毁。");
            return;
        }
        Object.Destroy(feather.gameObject);
    }
}