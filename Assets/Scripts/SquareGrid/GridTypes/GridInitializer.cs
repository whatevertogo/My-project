

using UnityEngine;

public abstract class GridInitializer : IGridInitializer
{
    public abstract void Init(GridType gridType, Renderer Renderer);

}