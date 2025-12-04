using System.Collections;
using UnityEngine;

public class Building : MonoBehaviour
{
    internal enum BuildingState
    {
        Waiting,
        Working
    }

    internal BuildingState state = BuildingState.Waiting;

    internal float productionTime;
    internal string buildingName = "DefaultBuildingName";

    private void Awake()
    {
        gameObject.name = $"{buildingName} x:{transform.position.x}|y:{transform.position.y}";
    }

    internal void ReceiveProdukt(GameObject receivedProduct)
    {
        if (state == BuildingState.Working) return;
        //if (null != receivedProduct)
        //    product = receivedProduct;
        StartCoroutine(Produce(receivedProduct));
    }
    internal void OutputProduct(GameObject finishedProduct)
    {
        if (state == BuildingState.Working) return;
        // Send Event
        // Find next building
        GameObject nextBuilding = new GameObject();
        if (null == nextBuilding)
            Debug.Log($"{gameObject.name} couldnt output finished Product!");

    }

    private IEnumerator Produce(GameObject receivedProduct)
    {
        state = BuildingState.Working;
        // Play animation
        // Play sound
        yield return new WaitForSeconds(productionTime);
        Debug.Log($"{gameObject.name} finished Production!");
        state = BuildingState.Waiting;

        GameObject finishedProduct = GetFinishedProduct(receivedProduct);

        OutputProduct(finishedProduct);
    }

    private void RecalculateStats()
    {

    }

    private GameObject GetFinishedProduct(GameObject receivedObjectsssssssss)
    {
        return new GameObject();
    }
}