using UnityEngine;

public class ItemInstance : MonoBehaviour
{
    [SerializeField] private ItemType itemType;
    [SerializeField] private float moveSpeed = 2f;
    internal float valueMultiplier = 1f;


    private Vector3 moveDirection;
    private bool isMoving;

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

    public void SetValueMultiplier(float multiplier)
    {
        valueMultiplier *= multiplier;
    }

}
