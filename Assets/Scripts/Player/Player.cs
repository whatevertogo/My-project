using CDTU.Utils;
using UnityEngine;
using System;
using System.Collections;
public class Player : Singleton<Player>
{
    private PlayerGridComponent playerGridComponent;
    private float moveDistance = 1f;
    [Tooltip("移动冷却时间")]
    [Range(0.1f, 5f)] // 限制冷却时间范围为 0.1 到 5 秒
    public float moveCooldown = 1f; // 可在 Inspector 设置冷却时间
    public float CooldownTimer { get; set; } = 1f;
    private bool isMoving = false; // 添加移动状态标志
    public bool IsMoving => isMoving; // 公开只读属性，允许外部访问移动状态
    [Tooltip("移动持续时间")]
    [Range(0.1f, 5f)]
    public float moveDuration = 1.0f;
    [ReadOnly]
    public Vector2 input;
    private SpriteRenderer spriteRenderer; // 添加精灵渲染器引用
    public PlayerAnimationController PlayerAnimationController;
    public static event Action OnNotify;


    protected override void Awake()
    {
        var Center = GridManager.Instance.GetGridCenter();

        this.transform.position = Center + new Vector3(0f, -0.1f, 0f);
        if (moveCooldown <= 0)
        {
            Debug.LogWarning("moveCooldown must be a positive value. Setting it to the default value of 1f.");
            moveCooldown = 1f;
        }

        // 尝试获取组件
        playerGridComponent = GetComponent<PlayerGridComponent>();
        if (playerGridComponent is null)
        {
            // 如果获取失败，则记录错误或警告
            Debug.LogError("PlayerGridComponent not found on the Player GameObject. Please add the component.");
            // 可以选择禁用脚本或返回，防止后续出错
            this.enabled = false;
            return;
        }
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        // 移除在此处打印坐标的逻辑，可移至 PlayerGridComponent 的 OnCellChanged 事件处理
        if (CooldownTimer > 0)
        {
            CooldownTimer -= Time.deltaTime;
            if (CooldownTimer < 0) CooldownTimer = 0; // 确保冷却时间不为负
        }

        // 只有冷却结束且不在移动中才允许尝试移动
        if (CooldownTimer <= 0 && !isMoving)
        {
            // 启动 TryMove 协程
            StartCoroutine(TryMove());
        }
    }

    /// <summary>
    /// 尝试根据玩家输入移动玩家角色。
    /// 该方法会处理玩家输入，检查移动的有效性，并在条件满足时执行移动操作。
    /// </summary>
    private IEnumerator TryMove()
    {
        isMoving = true; // 开始移动

        // 从游戏输入管理器获取玩家的移动输入
        input = GameInput.Instance.MoveInput;
        try
        {
            // 如果玩家没有输入移动指令，则直接返回，不执行后续移动逻辑
            if (input == Vector2.zero) yield break;

            // 禁止斜向移动：如果 x 和 y 同时不为 0，则直接返回
            if (input.x != 0 && input.y != 0) yield break;

            // 根据输入方向设置翻转
            if (input.x > 0)
            {
                gameObject.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f); // 向右移动时不翻转
            }
            else if (input.x < 0)
            {
                gameObject.transform.localScale = new Vector3(-0.2f, 0.2f, 0.2f); // 向左移动时翻转
            }

            // 将输入转为离散方向：左右或上下
            Vector2Int dir = new Vector2Int(
                input.x > 0 ? 1 : input.x < 0 ? -1 : 0,
                input.y > 0 ? 1 : input.y < 0 ? -1 : 0
            );

            // 如果转换后的方向向量为零向量，说明没有有效的移动方向，直接返回
            if (dir == Vector2Int.zero) yield break;

            // 确保当前格子存在
            if (playerGridComponent?.currentCell is null) yield break;

            // 计算目标格子世界坐标
            Vector3 originPos = playerGridComponent.currentCell.transform.position;
            Vector3 targetPos = originPos + new Vector3(dir.x * moveDistance, dir.y * moveDistance, 0f);

            // 检查目标格子是否存在和可移动
            SquareCell nextCell = GridManager.Instance.GetCell(targetPos);
            //todo-这里可以提示玩家格子不可移动
            Debug.Log($"{nextCell},{GridManager.Instance.AllDontMoveCells.Contains(nextCell)}");
            if (nextCell is null || GridManager.Instance.AllDontMoveCells.Contains(nextCell)) {
                //交互提示无法移动
                yield break;}

            // 等待平滑移动完成
            yield return StartCoroutine(SmoothMove(targetPos));
            
            // 移动完成后更新当前格子并重置冷却
            playerGridComponent.SetCurrentCell(nextCell);//原版
            //playerGridComponent.SetCurrentCell(nextCell,moveVector);//修改版
            MoveNotify();
            CooldownTimer = moveCooldown;
        }
        finally
        {
            isMoving = false; // 结束移动
        }
    }

    private IEnumerator SmoothMove(Vector3 targetPos)
    {
        Vector3 startPos = transform.position;
        float elapsedTime = 0f;

        PlayerAnimationController?.PlayJumpAnimation(); // 播放跳跃动画

        // 如果持续时间过短，直接设置位置以避免除零错误或瞬移
        if (moveDuration <= 0f)
        {
            transform.position = targetPos;
            yield break; // 退出协程
        }

        while (elapsedTime < moveDuration)
        {
            // 使用 Time.deltaTime 保证平滑过渡与帧率无关
            elapsedTime += Time.deltaTime;
            // 计算插值比例，并使用 Clamp01 确保比例在 0 到 1 之间
            float t = Mathf.Clamp01(elapsedTime / moveDuration);
            // 使用 Lerp 进行线性插值
            transform.position = Vector3.Lerp(startPos, targetPos, t);
            // 等待下一帧
            yield return null;
        }

        // 确保最终位置精确
        transform.position = targetPos;
    }
    void MoveNotify()
    {
        OnNotify?.Invoke(); // 通知所有监听者
    }
}