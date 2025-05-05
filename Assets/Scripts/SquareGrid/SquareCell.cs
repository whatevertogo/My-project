using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using HexGame.Harvest;

public class SquareCell : MonoBehaviour, ISquareCell, IInteract
{
    public SquareCoordinates Coordinates { get; set; }
    public readonly float duration = 1f;

    [ReadOnly] public GridType cellType = GridType.None; // 默认值为None
    public SpriteRenderer CellRenderer;

    private Harvestable harvestableComponent;

    /// <summary>
    /// 存储邻居的列表，包括自身和8个方向的邻居
    /// </summary>
    private readonly SquareCell[] neighborsAndSelf = new SquareCell[9];

    private ICellHoverHandler hoverHandler;

    private IGridTypeBehavior gridTypeBehavior;
    public GameObject chatObject; // 对话框对象
    public HarvestType harvestTypeWanted = HarvestType.None; // 需要的HarvestType


    public bool IsExplored { get; set; } = false; // 是否被探索过

    public bool IsPlaceable { get; set; } = true; // 是否可以放置物体

    public bool IsCreateChatBox { get; set; } = false; // 是否可以创建聊天框

    private void Awake()
    {
        CellRenderer = GetComponent<SpriteRenderer>();
        CellRenderer.sprite = Resources.Load<Sprite>("Images/Default");
        harvestableComponent = GetComponent<Harvestable>();
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

    //     #region 颜色管理
    //     public void SetColor(Color targetcolor, bool smooth)
    //     {
    //         if (smooth)
    //         {
    //             StartCoroutine(SmoothColorChange(targetColor: targetcolor));
    //         }
    //         else
    //         {
    //             currentColor = targetcolor;
    //             CellRenderer.color = targetcolor;
    //         }
    //     }

    //     private IEnumerator SmoothColorChange(Color targetColor)
    //     {
    //         Color startColor = currentColor;
    //         float elapsedTime = 0f;

    //         while (elapsedTime < duration)
    //         {
    //             elapsedTime += Time.deltaTime;
    //             float t = Mathf.Clamp01(elapsedTime / duration); // 归一化的时间值

    //             // 使用Mathf.SmoothStep使过渡更加平滑
    //             float smoothT = Mathf.SmoothStep(0f, 1f, t);

    //             // 计算当前颜色
    //             Color newColor = Color.Lerp(startColor, targetColor, smoothT);

    //             // 更新当前颜色和渲染器颜色
    //             currentColor = newColor;
    //             CellRenderer.color = newColor;

    //             // 等待下一帧
    //             yield return null;
    //         }

    //         // 确保最终颜色精确匹配目标颜色
    //         currentColor = targetColor;
    //         CellRenderer.color = targetColor;
    //     }

    //     public Color GetColor()
    //     {
    //         return currentColor;
    //     }


    // #endregion

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

    /// <summary>
    /// 设置网格类型
    ///  通过工厂模式获取不同的行为类
    ///  通过工厂模式获取不同的悬停Handler
    /// </summary>
    /// <param name="type"></param>
    public void SetGridType(GridType type)
    {
        //todo-激活不同type的逻辑
        this.cellType = type;
        hoverHandler = CellHoverHandlerFactory.GetHandler(cellType);
        SetHoverHandler(hoverHandler);
        gridTypeBehavior = GridTypeBehaviorFactory.GetBehavior(cellType);
        gridTypeBehavior.ApplyBehavior(this);
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
        harvestableComponent?.OnMouseDown();
        if (cellType == GridType.Feather)
        {
            HarvestManager.Instance.AddHarvest(HarvestType.Feather, 1);
            this.SetGridType(GridType.SimpleSquare);
            GridManager.Instance.AllDontMoveCells.Remove(this);
            var feather = transform.Find("Feather");
            var Feather = feather.gameObject;
            if (Feather is not null)
                Destroy(Feather);
        }
        if (cellType == GridType.BirdSquare && HarvestManager.Instance.GetResourceCount(harvestTypeWanted) > 0)
        {
            HarvestManager.Instance.ConsumeResource(harvestTypeWanted, 1);
            this.chatObject.transform.Find("ChatBox").Find("HarvestImage").GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Images/heart");
            HarvestManager.Instance.ConsumeResource(harvestTypeWanted, 1);//消耗资源
            GameManager.Instance.WinCore();//得分
        }
    }

    #endregion

    #region 鼠标悬停Handler

    public void SetHoverHandler(ICellHoverHandler handler)
    {
        hoverHandler = handler;
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

    #region 卡牌使用方法

    public void UseCard(Card card)
    {
        card.UseOn(this);
    }

    #endregion

    #region 鸟类格子随机需要的物品

    public void SetHarvestTypeWanted(HarvestType harvestType)
    {
        harvestTypeWanted = harvestType;
    }

    public HarvestType GetHarvestTypeWanted()
    {
        return harvestTypeWanted;
    }

    #endregion

    public void ShowWantedHarvest()
    {
        if (chatObject is not null)
        {
            var Image = new GameObject("HarvestImage");
            var spriteRenderer = Image.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = Resources.Load<Sprite>($"Images/zuoWu/{harvestTypeWanted}");
            Image.transform.SetParent(chatObject.transform, false);
            spriteRenderer.sortingLayerName = "Behavior";
            spriteRenderer.sortingOrder = 4; // 设置渲染顺序
            Image.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
            Image.transform.localPosition = new Vector3(0, 0, -0.1f);
        }
    }
}