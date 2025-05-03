


public class BirdSquareBehavior : IGridTypeBehavior
{
    public void ApplyBehavior(SquareCell cell)
    {
        cell.IsPlaceable = false;
        GridManager.Instance.AllBirdCells.Add(cell);
        GridManager.Instance.AllDontMoveCells.Add(cell);
    }
}