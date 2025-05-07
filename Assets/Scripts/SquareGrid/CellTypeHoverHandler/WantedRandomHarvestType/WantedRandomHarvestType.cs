using HexGame.Harvest;

public static class WantedRandomHarvestType
{
    public static HarvestType GetRandomBirdWantedType()
    {
        // 获取所有的 HarvestType 枚举值
        HarvestType[] harvestTypes = (HarvestType[])System.Enum.GetValues(typeof(HarvestType));

        // 过滤掉 None 类型
        var validHarvestTypes = System.Array.FindAll(harvestTypes, type => type != HarvestType.None);

        // 随机选择一个有效的 HarvestType
        int randomIndex = UnityEngine.Random.Range(0, validHarvestTypes.Length);
        return validHarvestTypes[randomIndex];
    }
}