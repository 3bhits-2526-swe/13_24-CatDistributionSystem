using UnityEngine;

public class BuildingPlacer : MonoBehaviour
{
    [SerializeField] private GameObject[] buildingPrefabs;

    private void Update() => HandlePlacementInput();

    private void HandlePlacementInput()
    {
        for (int i = 0; i < buildingPrefabs.Length; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
                GridBuildingSystem.Instance.StartPlacing(buildingPrefabs[i]);
        }

        // Right click to cancel
        if (Input.GetMouseButtonDown(1))
            GridBuildingSystem.Instance.CancelCurrentAction();

        // Place building on click
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = GridBuildingSystem.GetMouseWorldPosition();
            GridBuildingSystem.Instance.TryPlaceObject(mousePos);
        }
    }
}