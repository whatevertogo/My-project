using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    private PlayerAnimationControl playerAnimationControl;
    private Player player;
    [ReadOnly]
    public float WaitToPlayTime = 10f; // 等待播放时间
    private PlayerAnimationState lastState; // 记录上一次播放的动画

    private void Awake()
    {
        playerAnimationControl = GetComponent<PlayerAnimationControl>();
        if (playerAnimationControl is null)
        {
            Debug.LogError("PlayerAnimationControl 未初始化！");
            return;
        }
        player = GetComponent<Player>();
    }
    private void OnEnable()
    {
        playerAnimationControl.SetPlayerAnimation(0, PlayerAnimationState.Standby1, true);
    }

    private void Update()
    {
        if (player.IsMoving == true)
            WaitToPlayTime = 0; // 如果正在移动，则不等待播放动画

        if (WaitToPlayTime > 0)
        {
            WaitToPlayTime -= Time.deltaTime;
            if (WaitToPlayTime <= 0)
            {
                playerAnimationControl.SetPlayerAnimation(0, PlayerAnimationState.Standby2, false);
                // 重置等待时间
                WaitToPlayTime = Random.Range(7f, 10f);
                playerAnimationControl.SetPlayerAnimation(0, PlayerAnimationState.Standby1, true);
            }
        }
    }

    public void PlayJumpAnimation()
    {
        if (playerAnimationControl is null) return;
        Debug.Log($"{playerAnimationControl.skeletonAnimation.state.GetCurrent(0).Animation.Name}");
        playerAnimationControl.SetPlayerAnimation(0, PlayerAnimationState.Jump, false);
    }
}