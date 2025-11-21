using UnityEngine;
using static GridBuildingSystem;

public class Clickable : MonoBehaviour
{
    private GridBuildingSystem buildSystem;

    private void Awake() => buildSystem = GridBuildingSystem.Instance;

    private void OnMouseDown()
    {
        buildSystem.currentAction = MouseAction.None;
    }
}
