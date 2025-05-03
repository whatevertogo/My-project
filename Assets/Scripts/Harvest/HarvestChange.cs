using HexGame.Harvest;
using TMPro;
using UnityEngine;

public class HarvestChange : MonoBehaviour
{
    public HarvestType harvestType; // 资源类型
    public TextMeshProUGUI harvestText; // 显示资源数量的文本

    public void Start()
    {
        HarvestManager.Instance.OnHarvestChanged += OnHarvestChanged;
    }

    private void OnDestroy()
    {
        // 在对象销毁时取消订阅事件，防止内存泄漏和空引用异常
        if (HarvestManager.Instance != null)
        {
            HarvestManager.Instance.OnHarvestChanged -= OnHarvestChanged;
        }
    }

    private void OnDisable()
    {
        // 在对象被禁用时取消订阅事件
        if (HarvestManager.Instance != null)
        {
            HarvestManager.Instance.OnHarvestChanged -= OnHarvestChanged;
        }
    }

    private void OnEnable()
    {
        // 在对象被启用时重新订阅事件
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
        }
    }

}