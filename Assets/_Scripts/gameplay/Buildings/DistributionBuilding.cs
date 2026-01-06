using UnityEngine;

public class DistributionBuilding : BuildingBase
{
    private int truckloadSize = 3;
    private int currentLoad = 0;
    private float sellTimer = 0f;

    public DistributionBuilding(BuildingData data) : base(data)
    {
        data.maxInputs = 3;
        data.maxOutputs = 0;
        outputBuffer.capacity = 0;
    }

    public override void Process()
    {
        sellTimer += Time.deltaTime;

        while (inputBuffer.HasItems && currentLoad < truckloadSize)
        {
            var item = inputBuffer.RemoveItem();
            ProcessIncomingItem(item);
        }

        if (currentLoad >= truckloadSize && sellTimer >= 1f)
        {
            SellTruckload();
            sellTimer = 0f;
        }
    }

    private void ProcessIncomingItem(ItemInstance item)
    {
        currentLoad++;
        int value = item.GetTotalValue();

        ResourceManager.Instance.AddMoney(value);
        ResourceManager.Instance.AddExperience(value / 2);
    }

    private void SellTruckload()
    {
        int totalValue = currentLoad * 100;
        ResourceManager.Instance.AddMoney(totalValue);
        ResourceManager.Instance.AddExperience(currentLoad * 10);
        currentLoad = 0;
    }

    public override bool CanAcceptInput() => inputBuffer.CanAddItem();
    public override bool HasOutput() => false;

    public override void OnLevelUp(int globalLevel)
    {
        base.OnLevelUp(globalLevel);
        truckloadSize = 3 + (currentLevel * 2);
    }
}