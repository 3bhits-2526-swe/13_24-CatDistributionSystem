using UnityEngine;

public class BuildingGhost : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private GridBuildingSystem buildSystem;
    private Color originalColor;

    public void Initialize(GridBuildingSystem system)
    {
        buildSystem = system;
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
            spriteRenderer.color = buildSystem.validPlacementColor;
        }

        // Disable all components except renderer
        MonoBehaviour[] scripts = GetComponents<MonoBehaviour>();

        foreach (var script in scripts)
            if (script != this)
                script.enabled = false;

        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
            collider.enabled = false;
    }

    public void UpdateVisual(bool canPlace)
    {
        if (spriteRenderer != null)
            spriteRenderer.color = canPlace ? buildSystem.validPlacementColor : buildSystem.invalidPlacementColor;
    }
}