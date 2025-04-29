using System;
using UnityEngine;

public class PlayerGridComponent : MonoBehaviour
{
    [ReadOnly]
    public SquareCell currentCell;
    public event EventHandler<OnCellChangedEventArgs> OnCellChanged;

    public class OnCellChangedEventArgs : EventArgs
    {
        public SquareCell cell { get; }
        public Vector2 inputDirection { get; }
        public OnCellChangedEventArgs(SquareCell newCell)//旧版
        {
            cell = newCell;
        }
    }

    public void Start()
    {
        // 尝试获取当前格子
        currentCell = GridManager.Instance.GetCell(transform.position);
        if (currentCell is null)
        {
            Debug.LogError("Player is not on a valid cell. Please check the player's position.");
            return;
        }
        OnCellChanged?.Invoke(this, new OnCellChangedEventArgs(currentCell));
    }

    // 添加一个公共方法来设置当前格子并触发事件，由 Player.cs 调用
    public void SetCurrentCell(SquareCell cell)
    {
        if (cell != currentCell)
        {
            currentCell = cell;
            OnCellChanged?.Invoke(this, new OnCellChangedEventArgs(currentCell));
        }
    }
}