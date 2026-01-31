using UnityEngine;

public class Packager : MonoBehaviour
{
    [SerializeField] private PackagerRecipe recipe;
    [SerializeField] private BuildingInput input;
    [SerializeField] private BuildingOutput output;

    private int bufferedCount;
    private float timer;
    private bool isProcessing;

    private void Update()
    {
        TryConsumeInput();
        Process();
    }

    private void TryConsumeInput()
    {
        if (isProcessing)
            return;

        if (!input.TryConsume(out ItemInstance item))
            return;

        if (item.ItemType != recipe.Input)
            return;

        Destroy(item.gameObject);
        bufferedCount++;
    }

    private void Process()
    {
        if (isProcessing)
        {
            timer += Time.deltaTime;

            if (timer >= recipe.ProcessTime)
                Finish();

            return;
        }

        if (bufferedCount < 1)
            return;

        bufferedCount--;
        timer = 0f;
        isProcessing = true;
    }

    private void Finish()
    {
        isProcessing = false;

        ItemInstance item = Instantiate(
            recipe.Output,
            output.transform.position,
            Quaternion.identity
        );
        item.gameObject.SetActive(false);

        item.SetValueMultiplier(recipe.ValueMultiplier);
        output.TryOutput(item);
    }
}
