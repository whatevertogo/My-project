using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    private PlayerAnimationControl playerAnimationControl;

    public float WaitToPlayTime = 1f; // 等待播放时间
    private PlayerAnimationState lastState; // 记录上一次播放的动画

    private void Awake()
    {
        playerAnimationControl = GetComponent<PlayerAnimationControl>();
        if (playerAnimationControl == null)
        {
            Debug.LogError("PlayerAnimationControl 未初始化！");
        }
    }

    private void Update()
    {
        if (WaitToPlayTime > 0)
        {
            WaitToPlayTime -= Time.deltaTime;
            if (WaitToPlayTime <= 0)
            {
            }
        }
    }

    public void PlayJumpAnimation()
    {
        if (playerAnimationControl is null) return;
        playerAnimationControl.SetPlayerAnimation(0, PlayerAnimationState.Jump, false);
    }
}