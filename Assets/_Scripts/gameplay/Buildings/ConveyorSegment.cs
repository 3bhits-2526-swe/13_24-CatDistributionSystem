using System;
using UnityEngine;

public class ConveyorSegment : MonoBehaviour
{
    public static event Action ConveyorTopologyChanged;

    [SerializeField] private Transform entryPoint;
    [SerializeField] private Transform exitPoint;
    [SerializeField] private LayerMask conveyorLayer;
    [SerializeField] private float linkRadius = 0.2f;

    private ConveyorSegment nextSegment;
    private ItemInstance currentItem;

    private void RebuildLinks()
    {
        Debug.Log(gameObject.name + ": Rebuilding links");
        nextSegment = null;

        Collider2D[] hits = Physics2D.OverlapCircleAll(exitPoint.position, linkRadius, conveyorLayer);

        foreach (Collider2D hit in hits)
        {
            if (hit == null)
                continue;

            ConveyorSegment candidate = hit.GetComponent<ConveyorSegment>();

            if (candidate == null || candidate == this)
                continue;

            nextSegment = candidate;
            Debug.Log(gameObject.name + ": this is the candidate:" + candidate.gameObject.name);
            break;
        }
    }

    public bool CanAccept()
    {
        return currentItem == null;
    }

    public void Accept(ItemInstance item)
    {
        currentItem = item;
        item.transform.position = entryPoint.position;
        item.SetMoveDirection((exitPoint.position - entryPoint.position).normalized);
    }

    private void Update()
    {
        if (currentItem == null)
            return;

        if (Vector3.Distance(currentItem.transform.position, exitPoint.position) > 0.05f)
            return;

        if (nextSegment != null && nextSegment.CanAccept())
        {
            currentItem.SetMoveDirection((exitPoint.position - entryPoint.position).normalized);
            nextSegment.Accept(currentItem);
            currentItem = null;
        }
        else
        {
            currentItem.Stop();
        }
    }

    // Trigger Events

    private void OnEnable()
    {
        ConveyorTopologyChanged += RebuildLinks;
        UpdateTopology();
    }

    private void OnDisable() =>
        UpdateTopology();

    private void OnDestroy()
    {
        ConveyorTopologyChanged -= RebuildLinks;
        UpdateTopology();
    }

    private void TriggerTopologyEvent()
    {
        ConveyorTopologyChanged?.Invoke();
    }

    private void UpdateTopology()
    {
        CancelInvoke(nameof(TriggerTopologyEvent));
        Invoke(nameof(TriggerTopologyEvent), 0.1f);
    }

    private void OnDrawGizmosSelected()
    {
        if (exitPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(exitPoint.position, linkRadius);
        }
    }
}
