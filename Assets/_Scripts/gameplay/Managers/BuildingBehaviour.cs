using UnityEngine;

public class BuildingBehaviour : MonoBehaviour
{
    public BuildingBase buildingBase { get; private set; }
    public SpriteRenderer spriteRenderer { get; private set; }

    private float productionTimer = 0f;

    public void Initialize(BuildingBase building)
    {
        buildingBase = building;
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();

        if (building.data != null && building.data.sprite != null)
            spriteRenderer.sprite = building.data.sprite;
    }

    private void Update()
    {
        if (buildingBase == null) return;

        productionTimer += Time.deltaTime;
        float productionSpeed = buildingBase.data.baseProductionTime / buildingBase.currentLevel;

        if (productionTimer >= productionSpeed)
        {
            productionTimer = 0f;
            buildingBase.Process();
        }
    }

    private void OnMouseDown()
    {
        if (UIManager.Instance != null)
            UIManager.Instance.ShowBuildingInfo(this);
        Debug.Log("Clicked On Building: " + gameObject.name);
    }

    public void Rotate()
    {
        buildingBase.rotation = (buildingBase.rotation + 90) % 360;
        transform.rotation = Quaternion.Euler(0, 0, buildingBase.rotation);
    }
}