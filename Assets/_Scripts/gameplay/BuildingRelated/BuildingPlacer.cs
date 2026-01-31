using UnityEngine;

public class BuildingPlacer : MonoBehaviour
{
    //[SerializeField] private Camera cam;
    //[SerializeField] private BuildingGhost ghostPrefab;

    //private BuildingDefinition current;
    //private BuildingGhost ghostInstance;
    //private Vector2Int rotation = Vector2Int.one;

    //private void Update()
    //{
    //    if (current == null)
    //        return;

    //    UpdateGhost();

    //    if (Input.GetKeyDown(KeyCode.R))
    //        Rotate();

    //    if (Input.GetMouseButtonDown(0))
    //        TryPlace();

    //    if (Input.GetMouseButtonDown(1))
    //        Cancel();
    //}
    //[ContextMenu("BeginPlacement")]
    //public void BeginPlacement(BuildingDefinition definition)
    //{
    //    current = definition;
    //    ghostInstance = Instantiate(ghostPrefab);
    //}

    //private void UpdateGhost()
    //{
    //    Vector3 world = cam.ScreenToWorldPoint(Input.mousePosition);
    //    Vector2Int grid = GridManager.Instance.WorldToGrid(world);

    //    Vector2Int size = GetRotatedSize();
    //    bool valid = GridManager.Instance.CanPlace(grid, size);

    //    ghostInstance.transform.position = GridManager.Instance.GridToWorld(grid);
    //    ghostInstance.transform.localScale = new Vector3(size.x, size.y, 1);
    //    ghostInstance.SetValid(valid);
    //}

    //private void TryPlace()
    //{
    //    Vector3 world = cam.ScreenToWorldPoint(Input.mousePosition);
    //    Vector2Int grid = GridManager.Instance.WorldToGrid(world);
    //    Vector2Int size = GetRotatedSize();

    //    if (!GridManager.Instance.CanPlace(grid, size))
    //        return;

    //    GameObject obj = Instantiate(current.Prefab);
    //    BuildingBase building = obj.GetComponent<BuildingBase>();
    //    building.Place(grid);
    //}

    //private void Rotate()
    //{
    //    rotation = new Vector2Int(rotation.y, rotation.x);
    //}

    //private Vector2Int GetRotatedSize()
    //{
    //    return new Vector2Int(
    //        current.Size.x * rotation.x,
    //        current.Size.y * rotation.y
    //    );
    //}

    //private void Cancel()
    //{
    //    Destroy(ghostInstance.gameObject);
    //    current = null;
    //}
}
