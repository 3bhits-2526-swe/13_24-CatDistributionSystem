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

    [SerializeField] private GameObject outPutProduct;
    [SerializeField] private float productionTime;

    private void Awake()
    {
        // Set obj name
    }

    internal void ReceiveProdukt(GameObject receivedProduct)
    {
        if (state == BuildingState.Working) return;
        if (null != receivedProduct)
            product = receivedProduct;
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
        GetFinishedProduct(receivedProduct);
        OutputProduct();
    }
    
    private void RecalculateStats()
    {

    }

    private GameObject GetFinishedProduct()
    {

    }
}