using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingPaletteUI : MonoBehaviour
{
    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private Transform buttonParent;
    [SerializeField] private GameObject palettePanel;

    [SerializeField] private List<BuildingData> availableBuildings = new List<BuildingData>();

    [Header("Button Layout")]
    [SerializeField] private int yStart = 100;
    [SerializeField] private int yIncrement = -80;

    private List<BuildingButton> spawnedButtons = new List<BuildingButton>();
    private bool isPanelVisible = true;

    private void Start()
    {
        GenerateButtons();
        SetPanelVisible(isPanelVisible);
    }

    private void GenerateButtons()
    {
        ClearExistingButtons();

        float currentY = yStart;

        for (int i = 0; i < availableBuildings.Count; i++)
        {
            BuildingData buildingData = availableBuildings[i];
            if (buildingData == null) continue;

            GameObject buttonObject = Instantiate(buttonPrefab, buttonParent);
            RectTransform buttonRect = buttonObject.GetComponent<RectTransform>();

            buttonRect.anchoredPosition = new Vector2(0, currentY);

            BuildingButton buttonComponent = buttonObject.GetComponent<BuildingButton>();
            if (buttonComponent != null)
            {
                buttonComponent.Setup(buildingData);
                spawnedButtons.Add(buttonComponent);
            }

            currentY += yIncrement;
        }
    }

    private void ClearExistingButtons()
    {
        foreach (BuildingButton button in spawnedButtons)
        {
            if (button != null)
                Destroy(button.gameObject);
        }
        spawnedButtons.Clear();
    }

    public void SetPanelVisible(bool visible)
    {
        isPanelVisible = visible;
        if (palettePanel != null)
            palettePanel.SetActive(visible);
    }

    public void TogglePanel()
    {
        SetPanelVisible(!isPanelVisible);
    }

    public void AddBuildingToPalette(BuildingData buildingData)
    {
        if (!availableBuildings.Contains(buildingData))
        {
            availableBuildings.Add(buildingData);
            GenerateButtons();
        }
    }

    public void RemoveBuildingFromPalette(BuildingData buildingData)
    {
        if (availableBuildings.Contains(buildingData))
        {
            availableBuildings.Remove(buildingData);
            GenerateButtons();
        }
    }
}