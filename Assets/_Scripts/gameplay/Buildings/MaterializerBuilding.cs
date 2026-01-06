using UnityEngine;

public class MaterializerBuilding : BuildingBase
{
    private ItemData rawMaterial;

    public MaterializerBuilding(BuildingData data, ItemData outputItem) : base(data)
    {
        rawMaterial = outputItem;
        data.maxInputs = 0;
        data.maxOutputs = 1;
        inputBuffer.capacity = 0;
        outputBuffer.capacity = 1;
    }

    public override void Process()
    {
        if (!HasOutput()) return;

        if (outputBuffer.CanAddItem())
        {
            var newItem = new ItemInstance(rawMaterial);
            outputBuffer.AddItem(newItem);
        }
    }

    public override bool CanAcceptInput() => false;
    public override bool HasOutput() => outputBuffer.CanAddItem();
}