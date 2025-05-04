
using UnityEngine;

public class PlantedFeatherBehavior : IGridTypeBehavior
{
    public void ApplyBehavior(SquareCell cell)
    {
        cell.IsPlaceable = false;
        GridManager.Instance.AllDontMoveCells.Add(cell);
        var feather = new GameObject("Feather");
        var featherRenderer = feather.AddComponent<SpriteRenderer>();
        feather.transform.SetParent(cell.transform);
        feather.transform.localPosition = new Vector3(0, 0, 0);
        feather.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
        var featherSprite = Resources.Load<Sprite>("Images/Feather");
        featherRenderer.sprite = featherSprite;
    }
}