using UnityEngine;

public class ConnectionVisualizer : MonoBehaviour
{
    [SerializeField] private bool showConnections = true;
    [SerializeField] private Color connectionColor = Color.yellow;

    private void OnDrawGizmos()
    {
        if (!showConnections) return;

        BuildingBehaviour[] allBuildings = FindObjectsOfType<BuildingBehaviour>();

        foreach (BuildingBehaviour building in allBuildings)
        {
            if (building.buildingBase == null) continue;

            foreach (BuildingBehaviour output in building.buildingBase.outputConnections)
            {
                if (output != null)
                {
                    Gizmos.color = connectionColor;
                    Gizmos.DrawLine(
                        building.transform.position,
                        output.transform.position
                    );
                }
            }
        }
    }
}