using UnityEngine;

public class Packager : BuildingBase
{
    [SerializeField] private BuildingInput input;
    [SerializeField] private BuildingOutput output;

    public override float ProductionTime => 0;
    public override string StateText => "Packaging";
    public override int ActiveRecipeIndex => -1;
    public override void SetRecipe(int index) { }

    private void Update()
    {
        if (!output.IsFull && input.TryConsume(out ItemInstance item))
        {
            TransformItem(item);
        }
    }

    private void TransformItem(ItemInstance item)
    {
        item.ApplyPackaging();

        item.transform.position = output.transform.position;

        if (!output.TryOutput(item))
        {
            Destroy(item.gameObject);
        }
    }
}