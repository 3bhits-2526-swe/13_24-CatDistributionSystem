using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class BuildingButton : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private Image backgroundImage;

    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color hoverColor = Color.green;
    [SerializeField] private Color cannotAffordColor = Color.red;

    private BuildingData buildingData;
    private bool canAfford = false;

    public void Setup(BuildingData data)
    {
        buildingData = data;
        UpdateDisplay();
        UpdateAffordability();
    }

    private void UpdateDisplay()
    {
        if (buildingData == null) return;

        if (nameText != null)
            nameText.text = buildingData.buildingName;

        if (costText != null)
            costText.text = $"${buildingData.baseCost}";

        if (iconImage != null && buildingData.sprite != null)
            iconImage.sprite = buildingData.sprite;
    }

    private void Update()
    {
        UpdateAffordability();
    }

    private void UpdateAffordability()
    {
        if (buildingData == null || ResourceManager.Instance == null)
        {
            canAfford = false;
            return;
        }

        canAfford = ResourceManager.Instance.CanAfford(buildingData.baseCost);

        if (backgroundImage != null)
            backgroundImage.color = canAfford ? normalColor : cannotAffordColor;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (buildingData == null) return;

        Debug.Log($"BuildingButton clicked: {buildingData.buildingName}");

        if (!canAfford)
        {
            Debug.LogWarning($"Cannot afford {buildingData.buildingName}. Cost: ${buildingData.baseCost}, Current money: ${ResourceManager.Instance.GetMoney()}");
            return;
        }

        if (PlacementController.Instance == null)
        {
            Debug.LogError("PlacementController.Instance is null!");
            return;
        }

        Debug.Log($"Starting placement for {buildingData.buildingName}");
        PlacementController.Instance.StartPlacement(buildingData, PlacementMode.SelectPlace);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (backgroundImage != null)
            backgroundImage.color = canAfford ? hoverColor : cannotAffordColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (backgroundImage != null)
            backgroundImage.color = canAfford ? normalColor : cannotAffordColor;
    }
}