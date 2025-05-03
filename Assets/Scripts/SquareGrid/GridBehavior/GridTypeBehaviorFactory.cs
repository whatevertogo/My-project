
public static class GridTypeBehaviorFactory
{
    public static IGridTypeBehavior GetBehavior(GridType gridType)
    {
        return gridType switch
        {
            GridType.BirdSquare => new BirdSquareBehavior(),
            GridType.Feather => new PlantedFeatherBehavior(),
            GridType.SimpleSquare => new SimpleSquareBehavior(),
            GridType.PlantedTree => new PlantedTreeBehavior(),
            _ => throw new System.ArgumentOutOfRangeException(nameof(gridType), gridType, null)
        };
    }
}