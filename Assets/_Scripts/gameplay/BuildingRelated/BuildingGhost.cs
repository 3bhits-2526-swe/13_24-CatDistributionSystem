using UnityEngine;

public class BuildingGhost : MonoBehaviour
{
    [SerializeField] private SpriteRenderer rendererer;
    [SerializeField] private Color validColor = new(0, 1, 0, 0.5f);
    [SerializeField] private Color invalidColor = new(1, 0, 0, 0.5f);

    public void SetValid(bool valid)
    {
        rendererer.color = valid ? validColor : invalidColor;
    }
}
