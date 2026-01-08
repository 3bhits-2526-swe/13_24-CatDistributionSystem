using System.Collections.Generic;
using UnityEngine;

public class BuildingPaletteUI : MonoBehaviour
{
    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private RectTransform buttonParent;
    [SerializeField] private GameObject palettePanel;
    [SerializeField] private CanvasGroup paletteCanvasGroup;

    [SerializeField] private List<BuildingData> availableBuildings = new List<BuildingData>();

    [Header("Button Layout")]
    [SerializeField] private int yStart = -40;
    [SerializeField] private int yIncrement = -80;
    [SerializeField] private int xPosition = 0;

    private List<BuildingButton> spawnedButtons = new List<BuildingButton>();
    private bool isPaletteOpen = true;

    private void Start()
    {
        if (paletteCanvasGroup == null)
            paletteCanvasGroup = GetComponent<CanvasGroup>();

        InitializeButtons();
        SetPaletteVisibility(true);
    }

    private void InitializeButtons()
    {
        ClearExistingButtons();

        int currentY = yStart;

        foreach (BuildingData buildingData in availableBuildings)
        {
            if (buildingData == null) continue;

            GameObject buttonObject = Instantiate(buttonPrefab, buttonParent);
            RectTransform buttonRect = buttonObject.GetComponent<RectTransform>();

            buttonRect.anchoredPosition = new Vector2(xPosition, currentY);

            BuildingButton buildingButton = buttonObject.GetComponent<BuildingButton>();
            if (buildingButton != null)
            {
                buildingButton.Initialize(buildingData);
                spawnedButtons.Add(buildingButton);
            }

            currentY += yIncrement;
        }
    }

    private void ClearExistingButtons()
    {
        foreach (BuildingButton button in spawnedButtons)
        {
            if (button != null && button.gameObject != null)
                Destroy(button.gameObject);
        }
        spawnedButtons.Clear();
    }

    public void SetPaletteVisibility(bool visible)
    {
        isPaletteOpen = visible;

        if (palettePanel != null)
            palettePanel.SetActive(visible);

        if (paletteCanvasGroup != null)
        {
            paletteCanvasGroup.alpha = visible ? 1f : 0f;
            paletteCanvasGroup.blocksRaycasts = visible;
            paletteCanvasGroup.interactable = visible;
        }
    }

    public void TogglePalette()
    {
        SetPaletteVisibility(!isPaletteOpen);
    }

    public bool IsMouseOverPalette()
    {
        if (!isPaletteOpen) return false;

        RectTransform paletteRect = GetComponent<RectTransform>();
        if (paletteRect == null) return false;

        Vector2 localMousePosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            paletteRect,
            Input.mousePosition,
            Camera.main,
            out localMousePosition
        );
        Debug.Log("Miazu");
        return paletteRect.rect.Contains(localMousePosition);
    }
}