using Spine; // 引入 Spine 命名空间以使用 TrackEntry
using Spine.Unity; // 添加对 Spine.Unity 的引用
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    private PlayerAnimationControl playerAnimationControl;
    private Player player;

    [Tooltip("切换到 Standby2 需要的静止时间")]
    public float WaitDurationForStandby2 = 5f; // 例如，静止5秒后播放 Standby2
    [ReadOnly]
    [SerializeField] private float currentIdleTime = 0f; // 当前已静止的时间
    private bool isPlayingSpecialIdle = false; // 是否正在播放 Standby2 或 Jump

    private void Awake()
    {
        playerAnimationControl = GetComponent<PlayerAnimationControl>();
        if (playerAnimationControl is null)
        {
            Debug.LogError("PlayerAnimationControl 未初始化！请在 Inspector 中检查。");
            this.enabled = false; // 禁用此脚本以防后续错误
            return;
        }

        player = GetComponent<Player>();
        if (player is null)
        {
            Debug.LogError("Player 未初始化！请在 Inspector 中检查。");
            this.enabled = false;
            return;
        }

        // 订阅 Spine 动画完成事件
        if (playerAnimationControl.skeletonAnimation != null && playerAnimationControl.skeletonAnimation.state != null)
        {
            playerAnimationControl.skeletonAnimation.state.Complete += HandleAnimationComplete;
        }
        else
        {
            Debug.LogError("无法订阅动画完成事件，SkeletonAnimation 或其 state 未初始化。");
        }
    }

    private void OnDestroy()
    {
        // 组件销毁时取消订阅事件，防止内存泄漏
        if (playerAnimationControl?.skeletonAnimation?.state != null)
        {
            playerAnimationControl.skeletonAnimation.state.Complete -= HandleAnimationComplete;
        }
    }

    private void Start()
    {
        // 游戏开始时，如果玩家不在移动，则播放 Standby1
        if (!player.IsMoving)
        {
            PlayAnimation(PlayerAnimationState.Standby1, true);
        }
        ResetIdleTimer(); // 初始化计时器
    }

    private void Update()
    {
        if (player.IsMoving)
        {
            // 移动状态由 Player.cs 中的 SmoothMove 调用 PlayJumpAnimation() 处理
            ResetIdleTimer(); // 移动时重置静止计时器
            // isPlayingSpecialIdle = true; // Jump 动画开始时会设置
        }
        else // 玩家静止
        {
            // 只有在不在播放特殊动画（Standby2 或 Jump）时才计时
            if (!isPlayingSpecialIdle)
            {
                currentIdleTime += Time.deltaTime;

                // 检查是否达到播放 Standby2 的时间
                if (currentIdleTime >= WaitDurationForStandby2)
                {
                    isPlayingSpecialIdle = true; // 标记开始播放 Standby2
                    PlayAnimation(PlayerAnimationState.Standby2, false); // 播放 Standby2，不循环
                    // 无需在此处重置计时器，将在 HandleAnimationComplete 中处理
                }
                else
                {
                    // 确保在计时期间播放的是 Standby1
                    PlayAnimationIfNeeded(PlayerAnimationState.Standby1, true);
                }
            }
        }
    }

    /// <summary>
    /// 播放跳跃动画 (由 Player.cs 调用)
    /// </summary>
    public void PlayJumpAnimation()
    {
        if (playerAnimationControl is null) return;

        // Debug.Log("播放跳跃动画");
        isPlayingSpecialIdle = true; // 标记正在播放特殊动画
        PlayAnimation(PlayerAnimationState.Jump, false); // 播放 Jump，不循环
        ResetIdleTimer(); // 跳跃时重置静止计时器
    }

    /// <summary>
    /// 处理非循环动画播放完成事件
    /// </summary>
    private void HandleAnimationComplete(TrackEntry trackEntry)
    {
        // 过滤掉循环动画的 Complete 事件
        if (trackEntry.Loop) return;

        // 检查是哪个动画完成了
        Spine.Animation completedAnimation = trackEntry.Animation;
        if (completedAnimation == null) return;

        // 获取当前动画状态对应的 AnimationReferenceAsset
        // 使用新的公共方法获取资源
        AnimationReferenceAsset jumpAsset = playerAnimationControl.GetAnimationAsset(PlayerAnimationState.Jump);
        AnimationReferenceAsset standby2Asset = playerAnimationControl.GetAnimationAsset(PlayerAnimationState.Standby2);


        // 如果是 Jump 或 Standby2 完成
        if ((jumpAsset != null && completedAnimation == jumpAsset.Animation) ||
            (standby2Asset != null && completedAnimation == standby2Asset.Animation))
        {
            // Debug.Log($"{completedAnimation.Name} 动画播放完成");

            // 动画完成后，重置标志位
            isPlayingSpecialIdle = false;
            ResetIdleTimer(); // 重置计时器

            // 如果玩家当前是静止状态，则切换回 Standby1
            if (!player.IsMoving)
            {
                // Debug.Log("切换回 Standby1");
                PlayAnimation(PlayerAnimationState.Standby1, true);
            }
            // else: Player is still moving, Jump might be re-triggered or another logic handles it.
        }
    }

    /// <summary>
    /// 播放动画的核心方法
    /// </summary>
    private void PlayAnimation(PlayerAnimationState state, bool loop)
    {
        if (playerAnimationControl == null) return;

        // 使用新的公共方法获取资源
        AnimationReferenceAsset animationAsset = playerAnimationControl.GetAnimationAsset(state);
        if (animationAsset == null)
        {
            Debug.LogWarning($"动画资源 {state} 未在 PlayerAnimationControl 中设置或获取失败。");
            return;
        }

        // 检查是否需要播放（避免重复设置相同的动画）
        var currentTrack = playerAnimationControl.skeletonAnimation?.state?.GetCurrent(0);
        if (currentTrack != null && currentTrack.Animation == animationAsset.Animation && currentTrack.Loop == loop)
        {
            return; // 已经在播放此动画了
        }

        // Debug.Log($"设置动画: {state}, 循环: {loop}");
        playerAnimationControl.SetPlayerAnimation(0, state, loop);
    }

    /// <summary>
    /// 仅在当前动画不是目标动画时才播放
    /// </summary>
    private void PlayAnimationIfNeeded(PlayerAnimationState state, bool loop)
    {
        if (playerAnimationControl == null) return;

        // 使用新的公共方法获取资源
        AnimationReferenceAsset animationAsset = playerAnimationControl.GetAnimationAsset(state);
        if (animationAsset == null) return; // 如果资源不存在，则不执行任何操作

        var currentTrack = playerAnimationControl.skeletonAnimation?.state?.GetCurrent(0);
        // 仅当当前没有动画，或者当前动画不是目标动画时才播放
        if (currentTrack == null || currentTrack.Animation != animationAsset.Animation || currentTrack.Loop != loop)
        {
            // Debug.Log($"需要播放: {state}, 当前: {currentTrack?.Animation?.Name}");
            PlayAnimation(state, loop);
        }
    }


    /// <summary>
    /// 重置静止计时器
    /// </summary>
    private void ResetIdleTimer()
    {
        currentIdleTime = 0f;
    }
}