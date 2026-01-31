using System.Collections.Generic;
using UnityEngine;

public class BuildingOutput : MonoBehaviour
{
    [Header("Output Settings")]
    [SerializeField] private float checkRadius = 0.2f;
    [SerializeField] private LayerMask conveyorLayer;

    [Header("Buffer Settings")]
    [SerializeField] private int capacity = 5;

    private Queue<ItemInstance> buffer = new Queue<ItemInstance>();

    public bool IsFull => buffer.Count >= capacity;
    public bool HasItem => buffer.Count > 0;

    public bool TryOutput(ItemInstance item)
    {
        if (IsFull)
            return false;

        buffer.Enqueue(item);

        item.Stop();
        item.transform.position = transform.position;
        return true;
    }

    private void Update()
    {
        if (HasItem)
            TryPushToConveyor();
    }

    private void TryPushToConveyor()
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position, checkRadius, conveyorLayer);
        if (hit == null) return;

        ConveyorSegment conveyor = hit.GetComponent<ConveyorSegment>();

        if (conveyor != null && conveyor.CanAccept())
        {
            if (buffer.TryDequeue(out ItemInstance item))
            {
                item.gameObject.SetActive(true);
                conveyor.Accept(item);
            }
        }
    }
}
