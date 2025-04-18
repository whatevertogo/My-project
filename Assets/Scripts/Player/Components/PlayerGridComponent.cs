using System;
using UnityEngine;

public class PlayerGridComponent : MonoBehaviour
{
    [ReadOnly]
    public SquareCell currentCell;
    public event EventHandler<OnCellChangedEventArgs> OnCellChanged; // 事件回调

    #region 事件
    public class OnCellChangedEventArgs
    {
        public SquareCell cell;
        public OnCellChangedEventArgs(SquareCell cell)
        {
            this.cell = cell;
        }
    }
    #endregion

    private void Start()
    {
        // 初始化当前格子为玩家初始位置所在格子并对齐到格子中心
        var initCell = GridManager.Instance.GetCell(transform.position);
        if (initCell != null)
        {
            currentCell = initCell;
            transform.position = currentCell.transform.position;
            OnCellChanged?.Invoke(this, new OnCellChangedEventArgs(currentCell));
        }
    }

    private void Update()
    {
        // 获取玩家当前位置对应的格子
        var cell = GridManager.Instance.GetCell(transform.position); // 你需要有GridManagerInstance的引用
        if (cell is not null && cell != currentCell)
        {
            currentCell = cell;
            OnCellChanged?.Invoke(this, new OnCellChangedEventArgs(currentCell)); // 触发事件
        }
    }


}