using UnityEngine;

public class ItemInstance : MonoBehaviour
{
    [SerializeField] private ItemType itemType;
    [SerializeField] private float moveSpeed = 2f;
    internal float valueMultiplier = 1f;

    private Vector3 moveDirection;
    private bool isMoving;
    private bool isPackaged = false;

    public ItemType ItemType => itemType;

    public void SetMoveDirection(Vector3 direction)
    {
        moveDirection = direction.normalized;
        isMoving = true;
    }

    public void Stop()
    {
        isMoving = false;
    }

    private void Update()
    {
        if (!isMoving)
            return;

        transform.position += moveDirection * moveSpeed * Time.deltaTime;
    }

    public void ApplyPackaging()
    {
        if (isPackaged) return;

        isPackaged = true;
        valueMultiplier = itemType.ValueMultiplier;

        // Update the visual look
        SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();
        if (sr != null && itemType.PackagedSprite != null)
        {
            sr.sprite = itemType.PackagedSprite;
        }
    }

    public void SetValueMultiplier(float multiplier) => valueMultiplier = multiplier;
}
