using UnityEngine;
using System.Collections.Generic;

public class BuildPaletteUI : MonoBehaviour
{
    [SerializeField] private Transform contentRoot;
    [SerializeField] private BuildButtonUI buttonPrefab;
    [SerializeField] private List<BuildingType> buildingTypes;

    [SerializeField] private float yStart;
    [SerializeField] private float yOffset;

    private void Awake()
    {
        Populate();
    }

    private void Populate()
    {
        float currentY = yStart;

        foreach (var type in buildingTypes)
        {
            BuildButtonUI button = Instantiate(buttonPrefab, contentRoot);
            RectTransform rect = button.GetComponent<RectTransform>();

            rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, currentY);
            currentY -= yOffset;

            button.Bind(type);
        }
    }
}
