using System;
using UnityEngine;

public class PlayerGridComponent : MonoBehaviour
{
    public SquareCell currentCell;
    public Player player;
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

    public void SetPlayer(Player player)
    {
        this.player = player;
    }

    public Player GetPlayer()
    {
        return player;
    }

}