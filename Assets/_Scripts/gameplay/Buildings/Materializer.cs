using UnityEngine;

public class Materializer : BuildingBase
{
    [Header("Materializer")]
    [SerializeField] private ItemInstance itemPrefab;
    [SerializeField] private float interval = 1f;
    private BuildingOutput output;

    private float timer;

    public override float ProductionTime => interval;
    public override string StateText => "Producing";
    public override int ActiveRecipeIndex => -1;
    public override void SetRecipe(int index) { }

    private void Awake()
    {
        output = GetComponentInChildren<BuildingOutput>();
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer < interval)
            return;

        timer = 0f;

        ItemInstance item = Instantiate(itemPrefab, output.transform.position, Quaternion.identity);

        if (!output.TryOutput(item))
            Destroy(item.gameObject);
        else
            Debug.Log("Materializing Item");
    }
}
