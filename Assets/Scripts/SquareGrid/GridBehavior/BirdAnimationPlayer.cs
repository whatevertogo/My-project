using UnityEngine;
using System.Collections;

public class BirdAnimationPlayer : MonoBehaviour
{
    private Animator animator;
    private string stateNameA;
    private string stateNameB;
    private float delayAfterStateA;
    private Coroutine animationRoutine;

    void Awake()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            animator = gameObject.AddComponent<Animator>();
            Debug.LogWarning($"[{gameObject.name}] 未找到Animator组件，已添加。请分配一个Animator Controller。", this.gameObject);
        }
        // Animator没有与'playAutomatically'相当的功能。
        // 自动播放行为由Animator Controller的默认状态管理。
    }

    public void Initialize(string firstStateName, string secondStateName, float delay, RuntimeAnimatorController controller = null)
    {
        this.stateNameA = firstStateName;
        this.stateNameB = secondStateName;
        this.delayAfterStateA = delay;

        if (animator.runtimeAnimatorController == null && controller != null)
        {
            animator.runtimeAnimatorController = controller;
        }
        else if (animator.runtimeAnimatorController == null)
        {
            Debug.LogError($"[{gameObject.name}] Animator没有分配RuntimeAnimatorController。动画将不会播放。请在检视器中分配一个或在Initialize方法中分配。", this.gameObject);
            enabled = false;
            return;
        }

        if (string.IsNullOrEmpty(this.stateNameA) || string.IsNullOrEmpty(this.stateNameB))
        {
            Debug.LogError($"[{gameObject.name}] 动画状态名称未正确设置。stateNameA: '{this.stateNameA}', stateNameB: '{this.stateNameB}'。禁用播放器。", this.gameObject);
            enabled = false;
            return;
        }

        // 在播放前确保Animator准备就绪并且状态存在是一个好习惯，
        // 但在播放前直接验证状态存在的复杂性。
        // 我们依赖于Animator在运行时找不到状态时抛出错误。

        if (animationRoutine != null)
        {
            StopCoroutine(animationRoutine);
        }
        animationRoutine = StartCoroutine(PlayAnimationSequence());
    }

    private IEnumerator PlayAnimationSequence()
    {
        if (animator == null || animator.runtimeAnimatorController == null || string.IsNullOrEmpty(stateNameA) || string.IsNullOrEmpty(stateNameB))
        {
            Debug.LogError($"[{gameObject.name}] PlayAnimationSequence: Animator未正确配置或状态名称缺失。退出协程。", this.gameObject);
            yield break;
        }

        // 等待一帧以确保Animator完全初始化，特别是如果控制器刚刚分配。
        yield return null;

        while (true)
        {
            // --- 播放状态A ---
            animator.Play(stateNameA, 0, 0f); // 播放状态，在第0层，从开始

            // 等待动画开始并获取其长度
            yield return null;
            float stateALength = GetCurrentAnimatorStateLength(0);
            if (stateALength <= 0)
            { // 如果长度无效则回退
                Debug.LogWarning($"[{gameObject.name}] 状态 {stateNameA} 的长度为0或无效。等待1秒作为回退。", this.gameObject);
                stateALength = 1f;
            }
            yield return new WaitForSeconds(stateALength);

            yield return new WaitForSeconds(delayAfterStateA);

            // --- 播放状态B ---
            animator.Play(stateNameB, 0, 0f);

            yield return null;
            float stateBLength = GetCurrentAnimatorStateLength(0);
            if (stateBLength <= 0)
            { // 如果长度无效则回退
                Debug.LogWarning($"[{gameObject.name}] 状态 {stateNameB} 的长度为0或无效。等待1秒作为回退。", this.gameObject);
                stateBLength = 1f;
            }
            yield return new WaitForSeconds(stateBLength);

        }
    }

    private float GetCurrentAnimatorStateLength(int layerIndex = 0)
    {
        if (animator != null && animator.runtimeAnimatorController != null && animator.gameObject.activeInHierarchy && animator.enabled)
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(layerIndex);
            // 状态的长度可以直接获取。
            // 对于非循环动画，这是单次播放的持续时间。
            return stateInfo.length;
        }
        return 0f;
    }

    void OnDisable()
    {
        if (animationRoutine != null)
        {
            StopCoroutine(animationRoutine);
            animationRoutine = null;
        }
    }

}
