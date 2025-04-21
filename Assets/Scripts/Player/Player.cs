using CDTU.Utils;
using UnityEngine;
using System.Collections;

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
        if (moveCooldown <= 0)
        {
            Debug.LogWarning("moveCooldown must be a positive value. Setting it to the default value of 1f.");
            moveCooldown = 1f;
        }
        if (playerGridComponent == null)
        {
            playerGridComponent = GetComponent<PlayerGridComponent>();
            if (playerGridComponent == null) return; // 确保组件存在
        }
        else
        {
            Debug.LogWarning("PlayerGridComponent is not assigned. Please assign it in the Inspector.");
        }
    }


    private void Update()
    {
        if (playerGridComponent.currentCell != null)
        {
            Debug.Log(playerGridComponent.currentCell.Coordinates);
        }

        if (CooldownTimer > 0)
        {
            CooldownTimer -= Time.deltaTime;
            if (CooldownTimer < 0) CooldownTimer = 0; // 确保冷却时间不为负
        }

        // 只有冷却结束才允许移动
        if (CooldownTimer <= 0)
        {
            TryMove();
        }
    }

    /// <summary>
    /// 尝试根据玩家输入移动玩家角色。
    /// 该方法会处理玩家输入，检查移动的有效性，并在条件满足时执行移动操作。
    /// </summary>
    private void TryMove()
    {
        // 从游戏输入管理器获取玩家的移动输入
        Vector2 input = GameInput.Instance.MoveInput;
        // 如果玩家没有输入移动指令，则直接返回，不执行后续移动逻辑
        if (input == Vector2.zero) return;

        // 将输入转为离散方向：左右或上下
        // 根据输入的 x 和 y 分量，将其转换为 -1、0 或 1 的离散值，组成方向向量
        Vector2Int dir = new Vector2Int(Mathf.RoundToInt(input.x), Mathf.RoundToInt(input.y));
        // 如果转换后的方向向量为零向量，说明没有有效的移动方向，直接返回
        if (dir == Vector2Int.zero) return;

        // 确保当前格子存在
        // 使用空传播运算符检查玩家的格子组件及其当前所在格子是否为 null，如果为 null 则无法移动，直接返回
        if (playerGridComponent?.currentCell == null) return;

        // 计算目标格子世界坐标
        // 获取当前格子的世界坐标作为起始位置
        Vector3 originPos = playerGridComponent.currentCell.transform.position;
        // 根据移动方向和移动距离计算目标格子的世界坐标
        Vector3 targetPos = originPos + new Vector3(dir.x * moveDistance, 0, dir.y * moveDistance);

        // 检查目标格子是否存在
        // 调用网格管理器的方法，根据目标位置获取对应的格子对象
        SquareCell nextCell = GridManager.Instance.GetCell(targetPos);
        // 如果目标格子不存在，则无法移动，直接返回
        if (nextCell == null) return; // 检查目标格子是否存在

        // 平滑移动玩家到目标位置
        // 启动协程，让玩家从当前位置平滑移动到目标位置
        StartCoroutine(SmoothMove(targetPos));

        // 更新当前格子并重置冷却
        // 将玩家当前所在的格子更新为目标格子
        playerGridComponent.currentCell = nextCell;
        // 重置移动冷却计时器，防止玩家在冷却期间再次移动
        CooldownTimer = moveCooldown;
    }
    private IEnumerator SmoothMove(Vector3 targetPos)
    {
        Vector3 startPos = transform.position;
        float elapsedTime = 0f;
        float duration = moveCooldown; // 记录当前冷却时间，避免协程中途被修改

        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(startPos, targetPos, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPos; // Ensure final position is exact
    }
}