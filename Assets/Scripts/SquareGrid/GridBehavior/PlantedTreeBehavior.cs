using UnityEngine;

public class PlantedTreeBehavior : IGridTypeBehavior
{
    public void ApplyBehavior(SquareCell cell)
    {
        cell.IsPlaceable = false;
        GridManager.Instance.AllDontMoveCells.Add(cell);
        cell.harvestTypeWanted = RandomWantedHarvestType.GetRandomHarvestType();
        cell.chatObject = new GameObject("TreeChat");
        var spriteRenderer = cell.chatObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = RandomTree.GetRandomTree();
    }
}