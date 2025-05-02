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

    private void OnHarvestChanged(object sender, HarvestManager.OnHarvestChangedEventArgs e)
    {
        if (e.HarvestType == harvestType)
        {
            // 更新文本显示资源数量
            harvestText.text = e.Amount.ToString();
        }
    }

}