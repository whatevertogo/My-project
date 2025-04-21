using System;
using UnityEngine;

public class PlayerGridComponent : MonoBehaviour
{
    public SquareCell currentCell;
    public event EventHandler<OnCellChangedEventArgs> OnCellChanged;

    public class OnCellChangedEventArgs : EventArgs
    {
        public SquareCell cell { get; }
        public OnCellChangedEventArgs(SquareCell newCell)
        {
            cell = newCell;
        }
    }

    // 移除 Update 方法，格子更新逻辑已移至 Player.cs
    // private void Update()
    // {
    //     SquareCell newCell = GridManager.Instance.GetCell(transform.position);
    //     if (newCell != null && newCell != currentCell)
    //     {
    //         currentCell = newCell;
    //         OnCellChanged?.Invoke(this, new OnCellChangedEventArgs(currentCell));
    //     }
    // }

    // 添加一个公共方法来设置当前格子并触发事件，由 Player.cs 调用
    public void SetCurrentCell(SquareCell cell)
    {
        if (cell != currentCell)
        {
            currentCell = cell;
            OnCellChanged?.Invoke(this, new OnCellChangedEventArgs(currentCell));
            Debug.Log($"Player entered cell: {currentCell.Coordinates}"); // 可选：添加日志
        }
    }
}