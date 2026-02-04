using UnityEngine;

public class PlacementController : MonoBehaviour
{
    [SerializeField] private PlacementGhost ghostPrefab;
    private Camera cam;

    private PlacementGhost ghost;
    private Vector2Int rotation = Vector2Int.one;

    private void Awake()
    {
        cam = Camera.main;
    }

    private void Update()
    {
        if (!PlacementState.Instance.IsPlacing)
        {
            ClearGhost();
            return;
        }

        EnsureGhost();
        UpdateGhost();

        if (Input.GetKeyDown(KeyCode.R))
            Rotate();

        if (Input.GetMouseButtonDown(0))
            TryPlace();

        if (Input.GetMouseButtonDown(1))
            Cancel();
    }

    private void EnsureGhost()
    {
        if (ghost != null)
            return;

        ghost = Instantiate(ghostPrefab);
        ghost.GetComponent<SpriteRenderer>().sprite = PlacementState.Instance.Current.icon;
    }

    private void ClearGhost()
    {
        if (ghost == null)
            return;

        Destroy(ghost.gameObject);
        ghost = null;
    }

    private void UpdateGhost()
    {
        Vector3 world = cam.ScreenToWorldPoint(Input.mousePosition);
        Vector2Int grid = GridManager.Instance.WorldToGrid(world);

        Vector2Int size = GetRotatedSize();
        bool valid = GridManager.Instance.CanPlace(grid, size);

        ghost.transform.position = GridManager.Instance.GridToWorld(grid);
        ghost.transform.localScale = new Vector3(size.x, size.y, 1);
        ghost.SetValid(valid);
    }

    private void TryPlace()
    {
        BuildingType type = PlacementState.Instance.Current;

        Vector3 world = cam.ScreenToWorldPoint(Input.mousePosition);
        Vector2Int grid = GridManager.Instance.WorldToGrid(world);
        Vector2Int size = GetRotatedSize();

        if (!GridManager.Instance.CanPlace(grid, size))
            return;

        if (!MoneyManager.Instance.CanAfford(type.cost))
        {
            Debug.Log("Not enough money");
            return;
        }

        if (!MoneyManager.Instance.Spend(type.cost))
            return;

        GameObject obj = Instantiate(type.prefab);
        BuildingBase building = obj.GetComponent<BuildingBase>();
        building.Place(grid, size);

        PlacementState.Instance.CancelPlacement();
    }

    private void Rotate()
    {
        rotation = new Vector2Int(rotation.y, rotation.x);
    }

    private Vector2Int GetRotatedSize()
    {
        BuildingType type = PlacementState.Instance.Current;
        return new Vector2Int(
            type.size.x * rotation.x,
            type.size.y * rotation.y
        );
    }

    private void Cancel()
    {
        PlacementState.Instance.CancelPlacement();
    }
}
