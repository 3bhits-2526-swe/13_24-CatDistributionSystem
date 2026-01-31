using UnityEngine;

public class PlacementGhost : MonoBehaviour
{
    [SerializeField] private SpriteRenderer rendererer;
    [SerializeField] private Color validColor;
    [SerializeField] private Color invalidColor;

    public void SetValid(bool valid)
    {
        rendererer.color = valid ? validColor : invalidColor;
    }
}
