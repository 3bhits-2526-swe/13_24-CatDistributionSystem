using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BuildingInfoPanel : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private TextMeshProUGUI buildingNameText;
    [SerializeField] private TextMeshProUGUI buildingLevelText;
    [SerializeField] private TextMeshProUGUI buildingDescriptionText;
    [SerializeField] private Button upgradeButton;
    [SerializeField] private Button destroyButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private TextMeshProUGUI upgradeCostText;
    [SerializeField] private GameObject maxLevelIndicator;

    private BuildingBehaviour currentBuilding;

    private void Awake()
    {
        if (upgradeButton != null)
            upgradeButton.onClick.AddListener(OnUpgradeClicked);

        if (destroyButton != null)
            destroyButton.onClick.AddListener(OnDestroyClicked);

        if (closeButton != null)
            closeButton.onClick.AddListener(Hide);

        Hide();
    }

    public void Show(BuildingBehaviour building)
    {
        currentBuilding = building;
        UpdateDisplay();

        if (panel != null)
            panel.SetActive(true);
    }

    public void Hide()
    {
        if (panel != null)
            panel.SetActive(false);
        currentBuilding = null;
    }

    private void UpdateDisplay()
    {
        if (currentBuilding == null || currentBuilding.buildingBase == null) return;

        BuildingBase buildingBase = currentBuilding.buildingBase;
        BuildingData data = buildingBase.data;

        if (buildingNameText != null)
            buildingNameText.text = data.buildingName;

        if (buildingLevelText != null)
            buildingLevelText.text = $"Level {buildingBase.currentLevel}";

        if (buildingDescriptionText != null)
            buildingDescriptionText.text = GetDescription(data);

        bool isMaxLevel = buildingBase.currentLevel >= data.maxLevel;

        if (upgradeButton != null)
            upgradeButton.gameObject.SetActive(!isMaxLevel);

        if (maxLevelIndicator != null)
            maxLevelIndicator.SetActive(isMaxLevel);

        if (upgradeCostText != null && !isMaxLevel)
        {
            int upgradeCost = data.baseCost * (buildingBase.currentLevel + 1);
            upgradeCostText.text = $"Upgrade: ${upgradeCost}";
        }
    }

    private string GetDescription(BuildingData data)
    {
        switch (data.buildingType)
        {
            case BuildingType.Materializer: return "Produces raw materials";
            case BuildingType.Factory: return "Processes materials into products";
            case BuildingType.Packager: return "Packages products for shipping";
            case BuildingType.Distribution: return "Sells finished products";
            default: return "";
        }
    }

    private void OnUpgradeClicked()
    {
        if (currentBuilding == null || currentBuilding.buildingBase == null) return;

        BuildingBase buildingBase = currentBuilding.buildingBase;
        int upgradeCost = buildingBase.data.baseCost * (buildingBase.currentLevel + 1);

        if (ResourceManager.Instance != null && ResourceManager.Instance.CanAfford(upgradeCost))
        {
            ResourceManager.Instance.SpendMoney(upgradeCost);
            buildingBase.OnLevelUp(ResourceManager.Instance.GetLevel());
            UpdateDisplay();
        }
    }

    private void OnDestroyClicked()
    {
        if (currentBuilding != null && BuildingManager.Instance != null)
        {
            BuildingManager.Instance.RemoveBuilding(currentBuilding);
            Hide();
        }
    }
}