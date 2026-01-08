using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BuildingButton : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Canvas dragCanvas;

    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color hoverColor = new Color(0.8f, 1f, 0.8f, 1f);
    [SerializeField] private Color cannotAffordColor = new Color(1f, 0.8f, 0.8f, 1f);

    private BuildingData buildingData;
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    private Vector2 originalPosition;
    private GameObject dragObject;
    private bool isDragging = false;
    private bool isPointerOver = false;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        rectTransform = GetComponent<RectTransform>();

        if (dragCanvas == null)
            dragCanvas = GetComponentInParent<Canvas>();
    }

    public void Initialize(BuildingData data)
    {
        buildingData = data;
        UpdateVisuals();
        UpdateAffordability();
    }

    private void Update()
    {
        UpdateAffordability();
    }

    private void UpdateVisuals()
    {
        if (buildingData == null) return;

        if (nameText != null)
            nameText.text = buildingData.buildingName;

        if (costText != null)
            costText.text = $"${buildingData.baseCost}";

        if (iconImage != null && buildingData.sprite != null)
            iconImage.sprite = buildingData.sprite;
    }

    private void UpdateAffordability()
    {
        if (buildingData == null || backgroundImage == null) return;

        bool affordable = ResourceManager.Instance != null &&
                         ResourceManager.Instance.CanAfford(buildingData.baseCost);

        if (!isPointerOver)
            backgroundImage.color = affordable ? normalColor : cannotAffordColor;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isPointerOver = true;
        if (backgroundImage != null)
        {
            bool affordable = ResourceManager.Instance != null &&
                             ResourceManager.Instance.CanAfford(buildingData.baseCost);
            backgroundImage.color = affordable ? hoverColor : cannotAffordColor;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isPointerOver = false;
        UpdateAffordability();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (buildingData == null) return;
        if (ResourceManager.Instance == null) return;

        if (!ResourceManager.Instance.CanAfford(buildingData.baseCost))
        {
            Debug.LogWarning($"Cannot afford {buildingData.buildingName}. Cost: ${buildingData.baseCost}, Money: ${ResourceManager.Instance.GetMoney()}");
            return;
        }

        originalPosition = rectTransform.anchoredPosition;
        canvasGroup.alpha = 0.5f;

        CreateDragObject();
        isDragging = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging || dragObject == null) return;

        Vector2 screenPoint = eventData.position;
        Vector2 localPoint;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            dragCanvas.transform as RectTransform,
            screenPoint,
            dragCanvas.worldCamera,
            out localPoint))
        {
            dragObject.transform.localPosition = localPoint;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!isDragging) return;

        canvasGroup.alpha = 1f;
        rectTransform.anchoredPosition = originalPosition;

        if (dragObject != null)
        {
            Destroy(dragObject);
        }

        bool isOverUI = IsPointerOverUIElement();

        if (!isOverUI && buildingData != null && PlacementController.Instance != null)
        {
            PlacementController.Instance.StartPlacement(buildingData, PlacementMode.SelectPlace);
        }

        isDragging = false;
    }

    private void CreateDragObject()
    {
        if (dragObject != null)
            Destroy(dragObject);

        dragObject = new GameObject("DragPreview");
        dragObject.transform.SetParent(dragCanvas.transform, false);
        dragObject.transform.SetAsLastSibling();

        Image dragImage = dragObject.AddComponent<Image>();
        dragImage.sprite = buildingData.sprite;
        dragImage.raycastTarget = false;
        dragImage.color = new Color(1f, 1f, 1f, 0.7f);

        RectTransform dragRect = dragObject.GetComponent<RectTransform>();
        dragRect.sizeDelta = rectTransform.sizeDelta;
        dragRect.anchoredPosition = rectTransform.anchoredPosition;
    }

    private bool IsPointerOverUIElement()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        foreach (RaycastResult result in results)
        {
            if (result.gameObject == gameObject ||
                result.gameObject.transform.IsChildOf(transform))
                continue;

            if (result.gameObject.GetComponent<RectTransform>() != null)
                return true;
        }

        return false;
    }
}