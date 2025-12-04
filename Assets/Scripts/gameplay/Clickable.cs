using UnityEngine;
using static GridBuildingSystem;

public class Clickable : MonoBehaviour
{
    private GridBuildingSystem buildSystem;

    private void Awake() => buildSystem = GridBuildingSystem.Instance;

    [SerializeField] private MouseAction buildingType = MouseAction.None;

    private bool isDoubleMouseAction = false;

    private void OnMouseDown()
    {
        if (buildSystem.currentAction != MouseAction.None)
            isDoubleMouseAction = true;
        buildSystem.currentAction = MouseAction.None;
    }
    
}
