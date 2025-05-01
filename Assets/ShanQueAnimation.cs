using System;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;

public class ShanQueAnimation : MonoBehaviour
{
    [SerializeField] private SkeletonAnimation skeletonAnimation; // 角色动画组件
    public AnimationReferenceAsset PlayerStandby1; // 动画资源 
    public AnimationReferenceAsset PlayerStandby2; // 动画资源 
    public AnimationReferenceAsset PlayerJump; // 动画资源 

    public Dictionary<string, Action> PlayerAnimationDict = new()
    {

    };

    /// <summary>
    /// 设置玩家动画
    /// </summary>
    /// <param name="animation">要播放的动画引用资源</param>
    /// <param name="loop">是否循环播放，默认为false</param>
    /// <returns>None.</returns>
    /// <remarks>当skeletonAnimation为空时，该方法不会执行任何操作</remarks>
    public void SetPlayerAnimation(AnimationReferenceAsset animation, bool loop = false)
    {
        // 检查骨骼动画组件是否已初始化
        if (skeletonAnimation is null) return;

        // 应用指定动画到骨骼动画组件
        skeletonAnimation.state.SetAnimation(0, animation, loop);
    }







}
