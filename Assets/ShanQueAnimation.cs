using Spine.Unity;
using UnityEngine;

public class ShanQueAnimation : MonoBehaviour
{
    [SerializeField] private SkeletonAnimation skeletonAnimation; // 角色动画组件
    public AnimationReferenceAsset PlayerStandby1; // 动画资源 
    public AnimationReferenceAsset PlayerStandby2; // 动画资源 
    public AnimationReferenceAsset PlayerJump; // 动画资源 


    public void SetPlayerAnimation(AnimationReferenceAsset animation, bool loop = false)
    {
        if (skeletonAnimation is null) return;
        skeletonAnimation.state.SetAnimation(0, animation, loop); // 设置动画
    }

}
