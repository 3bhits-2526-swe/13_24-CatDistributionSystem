using UnityEngine;

public class BuildingInput : MonoBehaviour
{
    [SerializeField] private float checkRadius = 0.25f;
    [SerializeField] private LayerMask itemLayer;

    public bool TryConsume(out ItemInstance item)
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position, checkRadius, itemLayer);

        if (hit == null)
        {
            item = null;
            return false;
        }
        item = hit.GetComponent<ItemInstance>();
        return item != null;
    }
}
