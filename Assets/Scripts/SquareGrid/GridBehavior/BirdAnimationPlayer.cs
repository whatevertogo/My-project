using UnityEngine;
using System.Collections;

public class BirdAnimationPlayer : MonoBehaviour
{
    private Animation animComponent;
    private AnimationClip clipA;
    private AnimationClip clipB;
    private float delayAfterClipA = 8f;
    private Coroutine animationRoutine;

    void Awake()
    {
        animComponent = GetComponent<Animation>();
        if (animComponent == null)
        {
            animComponent = gameObject.AddComponent<Animation>();
        }
        // 我们将手动控制播放，所以禁用自动播放
        animComponent.playAutomatically = false;
    }

    /// <summary>
    /// 初始化动画播放器并开始播放序列
    /// </summary>
    /// <param name="firstClip">第一个播放的动画片段</param>
    /// <param name="secondClip">第二个播放的动画片段</param>
    /// <param name="delay">第一个动画播放完毕后的延迟时间</param>
    public void Initialize(AnimationClip firstClip, AnimationClip secondClip, float delay)
    {
        this.clipA = firstClip;
        this.clipB = secondClip;
        this.delayAfterClipA = delay;

        if (this.clipA == null || this.clipB == null)
        {
            Debug.LogError("动画片段未正确设置，无法初始化 BirdAnimationPlayer。", this.gameObject);
            enabled = false; // 如果未正确初始化，则禁用此组件
            return;
        }

        // 将动画片段添加到 Animation 组件中
        // 检查是否已存在同名片段，避免重复添加（尽管 AddClip 通常会处理覆盖）
        if (animComponent.GetClip(this.clipA.name) == null)
        {
            animComponent.AddClip(this.clipA, this.clipA.name);
        }
        if (animComponent.GetClip(this.clipB.name) == null)
        {
            animComponent.AddClip(this.clipB, this.clipB.name);
        }

        // 如果已有动画协程在运行，先停止它
        if (animationRoutine != null)
        {
            StopCoroutine(animationRoutine);
        }
        animationRoutine = StartCoroutine(PlayAnimationSequence());
    }

    private IEnumerator PlayAnimationSequence()
    {
        // 确保动画片段已加载
        if (clipA == null || clipB == null)
        {
            Debug.LogError("动画片段为空，无法播放动画序列。", this.gameObject);
            yield break;
        }

        while (true) // 无限循环播放动画序列
        {
            // 播放第一个动画 (clipA)
            Debug.Log($"播放动画: {clipA.name}", this.gameObject);
            animComponent.Play(clipA.name);
            // 等待第一个动画播放完成
            yield return new WaitForSeconds(clipA.length);

            // 第一个动画播放完成后的延迟
            Debug.Log($"动画 {clipA.name} 播放完毕，延迟 {delayAfterClipA} 秒。", this.gameObject);
            yield return new WaitForSeconds(delayAfterClipA);

            // 播放第二个动画 (clipB)
            Debug.Log($"播放动画: {clipB.name}", this.gameObject);
            animComponent.Play(clipB.name);
            // 等待第二个动画播放完成
            yield return new WaitForSeconds(clipB.length);
            Debug.Log($"动画 {clipB.name} 播放完毕，循环回第一个动画。", this.gameObject);
            // 循环由 while(true) 控制，将自动回到序列的开始
        }
    }

    void OnDisable()
    {
        // 当组件被禁用或对象被销毁时，停止协程以避免潜在问题
        if (animationRoutine != null)
        {
            StopCoroutine(animationRoutine);
            animationRoutine = null;
        }
    }
}