
using System.Collections.Generic;

public static class CellHoverHandlerFactory
{
    private static readonly Dictionary<GridType, ICellHoverHandler> handlers = new()
    {
        { GridType.BirdSquare, new BirdSquareHoverHandler() },
        { GridType.PlantedTree, new TreeHoverHandler() },
        { GridType.Feather, new FeatherHoverHandler() },
        { GridType.SimpleSquare, new DefaultHoverHandler() },
    };

    public static ICellHoverHandler GetHandler(GridType type)
    {
        if (handlers.TryGetValue(type, out var handler))
        {
            return handler;
        }

        return new DefaultHoverHandler(); // 返回默认处理器
    }

    // 如果需要动态添加或修改处理器
    public static void RegisterHandler(GridType type, ICellHoverHandler handler)
    {
        handlers[type] = handler;
    }
}
