using CDTU.Utils;
using UnityEngine;

public class Player : Singleton<Player>
{
    private PlayerGridComponent playerGridComponent;
    private float moveDistance = 1f;
    public float moveCooldown = 1f; // 可在 Inspector 设置冷却时间
    public float CooldownTimer { get; set; } = 0f;

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

        Vector3 moveDir = new Vector3(input.x, 0, input.y).normalized;
        transform.position += moveDir * moveDistance;

        CooldownTimer = moveCooldown; // 重置冷却
    }
}