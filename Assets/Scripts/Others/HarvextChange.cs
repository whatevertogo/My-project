using TMPro;
using UnityEngine;

public class HarvextChange : MonoBehaviour
{
    public HarvestType harvestType;
    public GameObject harvest;
    public TextMeshProUGUI harvestAmountText;

    public void Start()
    {
        HarvestManager.Instance.OnHarvestChanged += HandleHarvestChanged;
    }

    private void HandleHarvestChanged(object sender, HarvestManager.OnHarvestChangedEventArgs e)
    {
        if (e.HarvestType == harvestType)
        {
            harvestAmountText.text = e.Amount.ToString();
        }
    }
}
