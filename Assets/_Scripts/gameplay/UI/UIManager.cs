using UnityEngine;
using System;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] private BuildingPaletteUI buildingPalette;
    [SerializeField] private ResourceDisplayUI resourceDisplay;
    [SerializeField] private BuildingInfoPanel buildingInfoPanel;
    [SerializeField] private LevelUpCelebrationPanel levelUpPanel;

    [SerializeField] private KeyCode togglePaletteKey = KeyCode.B;
    [SerializeField] private KeyCode cancelPlacementKey = KeyCode.Escape;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Update()
    {
        if (Input.GetKeyDown(togglePaletteKey))
        {
            ToggleBuildingPalette();
        }

        if (Input.GetKeyDown(cancelPlacementKey))
        {
            CancelCurrentPlacement();
        }
    }

    public void ToggleBuildingPalette()
    {
        if (buildingPalette != null)
            buildingPalette.TogglePanel();
    }

    public void CancelCurrentPlacement()
    {
        if (PlacementController.Instance != null)
            PlacementController.Instance.CancelPlacement();

        if (buildingInfoPanel != null)
            buildingInfoPanel.Hide();
    }

    public void ShowBuildingInfo(BuildingBehaviour building)
    {
        if (buildingInfoPanel != null)
            buildingInfoPanel.Show(building);
    }

    public void HideBuildingInfo()
    {
        if (buildingInfoPanel != null)
            buildingInfoPanel.Hide();
    }

    public void ShowLevelUp(int newLevel)
    {
        if (levelUpPanel != null)
            levelUpPanel.ShowCelebration(newLevel);
    }
}