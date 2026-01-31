using UnityEngine;

public class Materializer : MonoBehaviour
{
    [SerializeField] private ItemInstance itemPrefab;
    [SerializeField] private float interval = 1f;
    private BuildingOutput output;

    private float timer;

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
