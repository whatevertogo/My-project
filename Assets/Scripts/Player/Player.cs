using System.ComponentModel;
using CDTU.Utils;
using UnityEngine;

public class Player : Singleton<Player>
{
    private PlayerGridComponent playerGridComponent;
    private float moveDistance = 1f;
    public float moveCooldown = 1f; // 可在 Inspector 设置冷却时间
    public float CooldownTimer { get; set; } = 0f;

    [ReadOnly]
    public Vector3 BeginPosition;

    private void Start()
    {
        BeginPosition = GridManager.Instance.GetGridCenter();
    }

    private void Update()
    {
        playerGridComponent = GetComponent<PlayerGridComponent>();

        if (playerGridComponent is not null && playerGridComponent.currentCell is not null)
        {
            Debug.Log(playerGridComponent.currentCell.Coordinates);
        }

        if (CooldownTimer > 0)
        {
            CooldownTimer -= Time.deltaTime;
        }

        // 只有冷却结束才允许移动
        if (CooldownTimer <= 0)
        {
            TryMove();
        }
    }

    private void TryMove()
    {
        Vector2 input = GameInput.Instance.MoveInput;
        if (input == Vector2.zero) return;

        // 将输入转为离散方向：左右或上下
        Vector2Int dir = new Vector2Int(
            input.x > 0 ? 1 : input.x < 0 ? -1 : 0,
            input.y > 0 ? 1 : input.y < 0 ? -1 : 0
        );
        if (dir == Vector2Int.zero) return;

        // 确保当前格子存在
        if (playerGridComponent?.currentCell == null) return;

        // 计算目标格子世界坐标
        Vector3 originPos = playerGridComponent.currentCell.transform.position;
        Vector3 targetPos = originPos + new Vector3(dir.x * moveDistance, 0, dir.y * moveDistance);

        // 检查目标格子是否存在
        var nextCell = GridManager.Instance.GetCell(targetPos);
        if (nextCell == null) return;

        // 移动玩家并重置冷却
        transform.position = targetPos;
        CooldownTimer = moveCooldown;
    }
}