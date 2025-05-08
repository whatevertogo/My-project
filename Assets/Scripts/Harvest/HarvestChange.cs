/* using System.Collections;
using HexGame.Harvest;
using TMPro;
using UnityEngine;

public class HarvestChange : MonoBehaviour
{
    public HarvestType harvestType; // 资源类型
    public TextMeshProUGUI harvestText; // 显示资源数量的文本
    private Color originalColor; // 保存原始颜色
    private string originalText; // 保存原始文本

    public void Start()
    {
        // 保存初始状态
        if (harvestText != null)
        {
            originalColor = harvestText.color;
            originalText = harvestText.text;
        }

        HarvestManager.Instance.OnHarvestChanged += OnHarvestChanged;
    }

    private void OnDestroy()
    {
        if (HarvestManager.Instance != null)
        {
            HarvestManager.Instance.OnHarvestChanged -= OnHarvestChanged;
        }
    }

    private void OnDisable()
    {
        if (HarvestManager.Instance != null)
        {
            HarvestManager.Instance.OnHarvestChanged -= OnHarvestChanged;
        }
    }

    private void OnEnable()
    {
        if (HarvestManager.Instance != null)
        {
            HarvestManager.Instance.OnHarvestChanged += OnHarvestChanged;
        }
    }

    private void OnHarvestChanged(object sender, HarvestManager.OnHarvestChangedEventArgs e)
    {
        if (e.HarvestType == harvestType)
        {
            // 更新文本显示资源数量
            harvestText.text = e.Amount.ToString();

            // 设置颜色
            harvestText.color = Color.yellow;

            // 设置粗体（通过富文本）
            harvestText.text = $"<b>{harvestText.text}</b>";

            // 设置字体红色
            Color c = harvestText.color;
            c.r = 1f;
            c.g = 0f;
            c.b = 0f;
            harvestText.color = c;

            // 启动协程在 2 秒后恢复原样
            StartCoroutine(ResetTextAfterDelay(2f));
        }
    }

    private IEnumerator ResetTextAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // 恢复原始颜色和文本
        if (harvestText != null)
        {
            harvestText.color = originalColor;
            harvestText.text = originalText;
        }
    }
} */
using System.Collections;
using HexGame.Harvest;
using TMPro;
using UnityEngine;
using DG.Tweening;

public class HarvestChange : MonoBehaviour
{
    public HarvestType harvestType;                // 资源类型
    public TextMeshProUGUI harvestText;             // 显示资源数量的文本
    public Color highlightColor = Color.red;        // 高亮颜色
    public float highlightDuration = 2f;            // 高亮持续时间
    public bool useBoldText = true;                 // 是否使用粗体
    public Ease colorEase = Ease.OutQuad;            // 颜色变化效果
    
    private Color originalColor;                     // 保存原始颜色
    private string originalText;                     // 保存原始文本

    void Start()
    {
        harvestText.color = Color.yellow;
        if (harvestText != null)
        {
            originalColor = harvestText.color;
            originalText = harvestText.text;
        }

        if (HarvestManager.Instance != null)
        {
            HarvestManager.Instance.OnHarvestChanged += OnHarvestChanged;
        }
    }

    void OnDestroy()
    {
        if (HarvestManager.Instance != null)
        {
            HarvestManager.Instance.OnHarvestChanged -= OnHarvestChanged;
        }
    }

    private void OnEnable()
    {
        if (HarvestManager.Instance != null)
        {
            HarvestManager.Instance.OnHarvestChanged += OnHarvestChanged;
        }
    }

    private void OnDisable()
    {
        if (HarvestManager.Instance != null)
        {
            HarvestManager.Instance.OnHarvestChanged -= OnHarvestChanged;
        }
    }

    private void OnHarvestChanged(object sender, HarvestManager.OnHarvestChangedEventArgs e)
    {
        if (e.HarvestType == harvestType)
        {
            harvestText.text = e.Amount.ToString();
            if (useBoldText)
            {
                harvestText.text = $"<b>{harvestText.text}</b>";
            }

            // 使用 DOTween 动画改变颜色
            harvestText.DOColor(highlightColor, 0.3f).SetEase(colorEase);

            StartCoroutine(ResetTextAfterDelay(highlightDuration));
        }
    }

    private IEnumerator ResetTextAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (harvestText != null)
        {
            // 恢复颜色平滑动画
            harvestText.DOColor(originalColor, 0.3f).SetEase(colorEase);
        }
    }
}
