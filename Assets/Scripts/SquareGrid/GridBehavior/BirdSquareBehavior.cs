


public class BirdSquareBehavior : IGridTypeBehavior
{
    public void ApplyBehavior(SquareCell cell)
    {
        cell.IsPlaceable = false;
        GridManager.Instance.AllBridCells.Add(cell);
        GridManager.Instance.AllDontMoveCells.Add(cell);
        cell.harvestTypeWanted = RandomWantedHarvestType.GetRandomHarvestType();
    }
}