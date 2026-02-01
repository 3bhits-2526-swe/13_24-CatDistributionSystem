using UnityEngine;


public abstract class BuildingBase : MonoBehaviour, IBuildingInspectable
{
    [SerializeField] private int gridID;
    [SerializeField] internal BuildingType buildingType;

    protected int level;
    public BuildingType Type => buildingType;
    public int Level => level;

    public abstract float ProductionTime { get; }
    public abstract string StateText { get; }
    public abstract int ActiveRecipeIndex { get; }
    public abstract void SetRecipe(int index);


    private void Start()
    {
        gameObject.name += gridID.ToString();
    }

    public void Place(Vector2Int gridPosition, Vector2Int size)
    {
        gridID = GridManager.Instance.Place(gridPosition, size);
        transform.position = GridManager.Instance.GridToWorld(gridPosition);
    }

    public void Remove()
    {
        GridManager.Instance.Remove(gridID);
        Destroy(gameObject);
    }
}
