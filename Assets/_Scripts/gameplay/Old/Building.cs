using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Building : MonoBehaviour
{
    private Collider2D buildingCollider;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private bool isInFreeMoveMode = false;

    private void Awake()
    {
        buildingCollider = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
    }

    private void Update()
    {
        if (isInFreeMoveMode)
        {
            UpdateFreeMove();
        }
    }

    public void OnPickedUp()
    {
        if (buildingCollider != null)
            buildingCollider.enabled = false;

        if (spriteRenderer != null)
            spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0.7f);
    }

    public void OnPlaced()
    {
        if (buildingCollider != null)
            buildingCollider.enabled = true;

        if (spriteRenderer != null)
            spriteRenderer.color = originalColor;

        isInFreeMoveMode = false;
    }

    public void OnCancelled()
    {
        if (buildingCollider != null)
            buildingCollider.enabled = true;

        if (spriteRenderer != null)
            spriteRenderer.color = originalColor;

        isInFreeMoveMode = false;
    }

    public void EnterFreeMoveMode()
    {
        isInFreeMoveMode = true;
        OnPickedUp();
    }

    private void UpdateFreeMove()
    {
        Vector2 mousePos = GridBuildingSystem.GetMouseWorldPosition();
        Vector2 snappedPos = GridBuildingSystem.Instance.snapToGrid ?
            SnapToGrid(mousePos) : mousePos;

        transform.position = snappedPos;

        // Update visual based on placement validity
        UpdatePlacementVisual(CanPlaceAt(snappedPos));

        // Check for placement click
        if (Input.GetMouseButtonDown(0))
        {
            if (CanPlaceAt(snappedPos))
                OnPlaced();
        }
    }

    public bool CanPlaceAt(Vector2 position)
    {
        // Temporarily enable collider for overlap check
        if (buildingCollider != null)
        {
            buildingCollider.enabled = true;
            Collider2D[] overlaps = Physics2D.OverlapBoxAll(
                position,
                buildingCollider.bounds.size,
                0
            );
            buildingCollider.enabled = false;

            foreach (var overlap in overlaps)
            {
                if (overlap.gameObject == gameObject)
                    continue;

                if (overlap.CompareTag("Building"))
                    return false;
            }
        }

        return true;
    }

    private void UpdatePlacementVisual(bool canPlace)
    {
        if (spriteRenderer == null) return;

        spriteRenderer.color = canPlace ?
            new Color(0, 1, 0, 0.7f) :
            new Color(1, 0, 0, 0.7f);
    }

    private Vector2 SnapToGrid(Vector2 position)
    {
        float gridSize = GridBuildingSystem.Instance.gridSize;
        return new Vector2(
            Mathf.Round(position.x / gridSize) * gridSize,
            Mathf.Round(position.y / gridSize) * gridSize
        );
    }
}