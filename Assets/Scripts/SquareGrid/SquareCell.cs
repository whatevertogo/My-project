using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

public class SquareCell : MonoBehaviour, ISquareCell, IInteract
{
    public SquareCoordinates Coordinates { get; set; }
    public Color currentColor = Color.black;
    public readonly float duration = 1f;

    [ReadOnly]
    public GridType cellType = GridType.None; // 默认值为None
    public SpriteRenderer CellRenderer;
    /// <summary>
    /// 存储邻居的列表，包括自身和8个方向的邻居
    /// </summary>
    private readonly SquareCell[] neighborsAndSelf = new SquareCell[9];

    private ICellHoverHandler hoverHandler;

    public bool IsExplored { get; set; } = false; // 是否被探索过

    public bool IsPlaceable { get; set; } = true; // 是否可以放置物体


    private void Awake()
    {
        CellRenderer = GetComponent<SpriteRenderer>();
        CellRenderer.sprite = Resources.Load<Sprite>("Images/Default");
    }

    private void Start()
    {
        // 初始化Hover控制器
        hoverHandler = CellHoverHandlerFactory.GetHandler(cellType);
    }

    // 存储邻居的列表
    public Vector3 GetWorldPosition()
    {
        return Coordinates.ToWorldPosition();
    }

    public void Init(SquareCoordinates coordinates)
    {
        this.Coordinates = coordinates;
    }
    public SquareCoordinates GetCoordinates()
    {
        return Coordinates;
    }

    #region 颜色管理
    public void SetColor(Color targetcolor, bool smooth)
    {
        if (smooth)
        {
            StartCoroutine(SmoothColorChange(targetColor: targetcolor));
        }
        else
        {
            currentColor = targetcolor;
            CellRenderer.color = targetcolor;
        }
    }

    private IEnumerator SmoothColorChange(Color targetColor)
    {
        Color startColor = currentColor;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration); // 归一化的时间值

            // 使用Mathf.SmoothStep使过渡更加平滑
            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            // 计算当前颜色
            Color newColor = Color.Lerp(startColor, targetColor, smoothT);

            // 更新当前颜色和渲染器颜色
            currentColor = newColor;
            CellRenderer.color = newColor;

            // 等待下一帧
            yield return null;
        }

        // 确保最终颜色精确匹配目标颜色
        currentColor = targetColor;
        CellRenderer.color = targetColor;
    }

    public Color GetColor()
    {
        return currentColor;
    }


    #endregion

    #region 邻居方法
    /// <summary>
    /// 设置指定方向的邻居格子
    /// </summary>
    /// <param name="direction">邻居的方向</param>
    /// <param name="neighbor">要设置的邻居格子</param>
    public void SetNeighbor(SquareDirection direction, SquareCell neighbor)
    {
        int index = (int)direction;
        if (index >= 0 && index < neighborsAndSelf.Length)
        {
            neighborsAndSelf[index] = neighbor;
        }
        else
        {
            Debug.LogError($"无效的方向: {direction}，方向值应该在0到{neighborsAndSelf.Length - 1}之间");
        }
    }

    /// <summary>
    /// 获取指定方向的邻居格子
    /// </summary>
    /// <param name="direction">要获取的邻居方向</param>
    /// <returns>指定方向的邻居格子，如果方向无效或邻居不存在则返回null</returns>
    public SquareCell GetNeighbor(SquareDirection direction)
    {
        if (TryGetNeighbor(direction, out SquareCell neighbor))
        {
            return neighbor;
        }
        return null;
    }

    /// <summary>
    /// 获取所有有效的邻居（不包括null值）
    /// </summary>
    public List<SquareCell> GetNeighborsAndSelf()
    {
        return neighborsAndSelf.Where(n => n != null).ToList();
    }

    /// <summary>
    /// 获取指定方向的邻居，如果不存在则返回false
    /// </summary>
    public bool TryGetNeighbor(SquareDirection direction, out SquareCell neighbor)
    {
        int index = (int)direction;
        if (index >= 0 && index < neighborsAndSelf.Length && neighborsAndSelf[index] != null)
        {
            neighbor = neighborsAndSelf[index];
            return true;
        }
        neighbor = null;
        return false;
    }

    /// <summary>
    /// 获取以当前格子为中心的九宫格范围内所有有效的格子
    /// </summary>
    public List<SquareCell> GetSurroundingCells()
    {
        var validCells = new List<SquareCell> { this }; // 添加中心格子（自身）

        // 遍历所有可能的方向（除了Center）
        foreach (SquareDirection dir in System.Enum.GetValues(typeof(SquareDirection)))
        {
            if (dir == SquareDirection.Center) continue;

            if (TryGetNeighbor(dir, out SquareCell neighbor))
            {
                validCells.Add(neighbor);
            }
        }

        return validCells;
    }
    #endregion

    #region 运算符重载
    public override bool Equals(object obj)
    {
        if (obj is SquareCell other)
        {
            return Coordinates.Equals(other.Coordinates);
        }
        return false;
    }

    public override int GetHashCode()
    {
        return Coordinates.GetHashCode();
    }

    public override string ToString()
    {
        return $"SquareCell({Coordinates})";
    }

    #endregion

    #region 网格Type方法
    public void SetGridType(GridType type)
    {
        cellType = type;
        // 如果type不是SimpleSquare，则设置不能放置
        //todo-这里可以添加更多的类型判断逻辑，以后多了就用switch语句
        if (type != GridType.SimpleSquare)
        {
            IsPlaceable = false;
            //todo-调试用，后面可以删除
            if (!GridManager.Instance.AllCantMoveCells.Contains(this))
                GridManager.Instance.AllCantMoveCells.Add(this);
        }
        else
        {
            IsPlaceable = true;
        }

    }
    public GridType GetGridType()
    {
        return cellType;
    }
    public void ResetGridType()
    {
        cellType = GridType.SimpleSquare;
    }
    #endregion

    #region 交互方法
    public void Interact()
    {
        Debug.Log($"与格子 {this.Coordinates} 交互！");
    }
    #endregion

    #region 鼠标悬停Handler
    public void SetHoverHandler(ICellHoverHandler handler)
    {
        hoverHandler = handler;
    }
    public ICellHoverHandler GetHoverHandler()
    {
        return hoverHandler;
    }

    public void OnHoverEnter()
    {
        if (IsExplored == false) return;
        hoverHandler?.OnHoverEnter(this);
    }

    public void OnHoverExit()
    {
        if (IsExplored == false) return;
        hoverHandler?.OnHoverExit(this);
    }
    #endregion



}
