using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;

/// <summary>
/// 玩家动画控制器
/// </summary>
public class PlayerAnimationControl : MonoBehaviour
{
    [SerializeField] private SkeletonAnimation skeletonAnimation; // 角色动画组件
    public AnimationReferenceAsset PlayerStandby1; // 动画资源
    public AnimationReferenceAsset PlayerStandby2; // 动画资源
    public AnimationReferenceAsset PlayerJump; // 动画资源

    [Header("混合时间配置")]
    public AnimationMixConfig mixConfig; // 混合时间配置类

    private Dictionary<PlayerAnimationState, AnimationReferenceAsset> playerAnimationDict; // 动画字典

    private void Awake()
    {
        InitializeAnimationDictionary();
        InitializeMixSettings();
    }

    private void InitializeAnimationDictionary()
    {
        playerAnimationDict = new Dictionary<PlayerAnimationState, AnimationReferenceAsset>
        {
            { PlayerAnimationState.Standby1, PlayerStandby1 },
            { PlayerAnimationState.Standby2, PlayerStandby2 },
            { PlayerAnimationState.Jump, PlayerJump }
        };
    }

    private void InitializeMixSettings()
    {
        if (mixConfig is null || skeletonAnimation is null) return;

        foreach (var mix in mixConfig.MixSettings)
        {
            skeletonAnimation.state.Data.SetMix(mix.From, mix.To, mix.Duration);
        }
    }

    /// <summary>
    /// 设置玩家动画
    /// </summary>
    /// <param name="trackIndex">轨道索引</param>
    /// <param name="animationType">要播放的动画类型</param>
    /// <param name="loop">是否循环播放</param>
    public void SetPlayerAnimation(int trackIndex, PlayerAnimationState animationType, bool loop = false)
    {
        if (skeletonAnimation is null)
        {
            Debug.LogError("SkeletonAnimation 未初始化！");
            return;
        }

        if (playerAnimationDict.TryGetValue(animationType, out var animation))
        {
            skeletonAnimation.state.SetAnimation(trackIndex, animation, loop);
        }
        else
        {
            Debug.LogWarning($"动画类型 {animationType} 没有对应的动画资源！");
        }
    }
}
