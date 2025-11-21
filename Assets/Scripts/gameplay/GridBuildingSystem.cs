using UnityEngine;

public class GridBuildingSystem : MonoBehaviour
{
    public static GridBuildingSystem Instance { get; private set; }

    internal enum State
    {
        View,
        Place
    }

    public enum MouseAction
    {
        None,
        PickUp_Factory,
        PickUp_Packager,
        PickUp_Distribution,
        Placed
    }

    internal MouseAction currentAction = MouseAction.None;

    private void Awake()
    {
        Instance = this;
    }
}
