using UnityEngine;

public class PlacementState : MonoBehaviour
{
    public static PlacementState Instance { get; private set; }

    public bool IsPlacing => current != null;
    public BuildingType Current => current;

    private BuildingType current;

    private void Awake()
    {
        Instance = this;
    }

    public void BeginPlacement(BuildingType type)
    {
        current = type;
    }

    public void CancelPlacement()
    {
        current = null;
    }
}
