using UnityEngine;

public class BuildingBase : MonoBehaviour
{
    [SerializeField] private int gridID;

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
