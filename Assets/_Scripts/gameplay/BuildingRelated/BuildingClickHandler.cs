using UnityEngine;

public class BuildingClickHandler : MonoBehaviour
{
    private IBuildingInspectable inspectable;

    private void Awake()
    {
        inspectable = GetComponent<IBuildingInspectable>();
    }

    private void OnMouseDown()
    {
        if (inspectable == null)
            return;

        BuildingInfoPanel.Instance.Show(inspectable);
    }
}
