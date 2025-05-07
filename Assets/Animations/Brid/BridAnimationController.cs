using System.Collections;
using UnityEngine;
using Spine.Unity;

public class BirdAnimationController : MonoBehaviour
{
    private SkeletonAnimation skeletonAnimation;
    private bool isChangingAnimation = false;
    private float idleTime = 5f; // 默认播放第一个动画的时间
    private float animationSwitchInterval = 8f; // 每隔多久切换一次动画序列

    public void Initialize(SkeletonAnimation animation)
    {
        this.skeletonAnimation = animation;
        StartCoroutine(AnimationLoop());
    }

    private IEnumerator AnimationLoop()
    {
        while (true)
        {
            // 等待一段时间后切换到动画2
            yield return new WaitForSeconds(animationSwitchInterval);

            if (!isChangingAnimation)
            {
                StartCoroutine(PlayAnimationSequence());
            }
        }
    }

    private IEnumerator PlayAnimationSequence()
    {
        isChangingAnimation = true;

        // 切换到动画2
        skeletonAnimation.skeletonDataAsset = Resources.Load<SkeletonDataAsset>("Birds/YaQue/ReferenceAssets/shanQue_Standby2");
        skeletonAnimation.Initialize(true); // 重新初始化骨骼动画

        // 获取动画时长，如果无法获取则使用默认时长
        float animationDuration = skeletonAnimation.AnimationState.GetCurrent(0)?.Animation?.Duration ?? 2f;
        yield return new WaitForSeconds(animationDuration);

        // 动画2播放完后，切回动画1
        skeletonAnimation.skeletonDataAsset = Resources.Load<SkeletonDataAsset>("Birds/YaQue/ReferenceAssets/shanQue_Standby1");
        skeletonAnimation.Initialize(true); // 重新初始化骨骼动画

        isChangingAnimation = false;
    }
}