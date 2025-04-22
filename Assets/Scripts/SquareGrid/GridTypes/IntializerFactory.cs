using System.Collections.Generic;

public static class InitializerFactory
{
    public static readonly Dictionary<GridType, IGridInitializer> Initializers = new Dictionary<GridType, IGridInitializer>
    {
        { GridType.SimpleSquare, new SimpleSquareInitializer() },
        { GridType.BirdSquare, new BridSquareInitializer() }
    };

    public static IGridInitializer GetGridInitializer(GridType gridType)
    {
        if (Initializers.TryGetValue(gridType, out var initializer))
        {
            return initializer;
        }
        else
        {
            throw new System.Exception($"没有找到对应的网格初始化器: {gridType}");
        }
    }
}