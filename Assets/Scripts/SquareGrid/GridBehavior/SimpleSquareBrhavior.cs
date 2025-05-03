


public class SimpleSquareBehavior : IGridTypeBehavior
{
    public void ApplyBehavior(SquareCell cell)
    {
        cell.IsPlaceable = true;
    }
}